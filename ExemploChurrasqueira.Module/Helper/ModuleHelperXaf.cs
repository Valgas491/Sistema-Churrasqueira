using DevExpress.ExpressApp;
using ExemploChurrasqueira.Module.BusinessObjects;
using ExemploChurrasqueira.Module.BusinessObjects.NoPer;
using ExemploChurrasqueira.Module.BusinessObjects.Per;
using Newtonsoft.Json;
using RestSharp;

namespace ExemploChurrasqueira.Module.Helper
{
    public class ModuleHelperXaf
    {
        public static void ConfigurarSocios(object sender, ObjectsGettingEventArgs evento, IObjectSpace objectSpace)
        {
            try
            {
                if (evento.ObjectType != typeof(Socio))
                    return;

                Parametros parametrosServidor = objectSpace.GetObjects<Parametros>().FirstOrDefault();
                if (parametrosServidor is null)
                    throw new Exception("Erro ao buscar parâmetros do servidor");


                var endereco = $"{parametrosServidor.Endereco}/api/Pessoa/socios";
                var client = new RestClient(endereco);
                var requisicao = new RestRequest(endereco, Method.Get);
                requisicao.AddHeader("Cache-Control", "no-cache");
                requisicao.AddHeader("Content-Type", "application/json");

                var response = Task.Run(async () => await client.ExecuteAsync(requisicao)).Result;

                if (!response.IsSuccessful)
                    throw new Exception("ERRO");

                var sociosJson = response.Content;
                var socios = JsonConvert.DeserializeObject<List<Socio>>(sociosJson);

                var objects = socios.Select(s => new Socio
                {
                    Id = s.Oid,
                    Nome = s.Nome,
                    Npf = s.Npf
                }).ToList();

                evento.Objects = objects;
            }
            catch (Exception error)
            {

                Console.WriteLine(error.Message);
            }
        }

    }
}