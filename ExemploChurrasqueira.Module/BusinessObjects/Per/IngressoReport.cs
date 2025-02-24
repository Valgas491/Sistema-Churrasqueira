using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraReports.UI;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    public class IngressoReport : XtraReport
    {
        private TopMarginBand topMarginBand1;
        private DetailBand detailBand1;
        private BottomMarginBand bottomMarginBand1;

        public IngressoReport()
        {
            ConfigurarPagina();
            CriarDetalhes();
        }

        #region Métodos Da Criação De Página
        private void ConfigurarPagina()
        {
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Landscape = false;
        }

        private void CriarDetalhes()
        {
            DetailBand detailBand = new DetailBand { HeightF = 260 };
            this.Bands.Add(detailBand);
            AdicionarBordas(detailBand);
            AdicionarLogos(detailBand);
            AdicionarTitulo(detailBand);
            AdicionarCodigoQR(detailBand);
            AdicionarCamposDeTexto(detailBand);
            AdicionarImagemDeFundo(detailBand);
        }

        private void AdicionarBordas(DetailBand detailBand)
        {
            XRPanel borderPanel = new XRPanel
            {
                BoundsF = new RectangleF(5, 5, 580, 260),
                BorderWidth = 2,
                Borders = BorderSide.All,
                BorderColor = Color.Black
            };
            detailBand.Controls.Add(borderPanel);
        }

        private void AdicionarLogos(DetailBand detailBand)
        {
            XRPictureBox logo = new XRPictureBox
            {
                ImageUrl = "https://cdn-icons-png.flaticon.com/512/67/67827.png",
                BoundsF = new RectangleF(450, 150, 100, 100),
                Sizing = ImageSizeMode.StretchImage
            };
            detailBand.Controls.Add(logo);

            XRPictureBox logoMinas = new XRPictureBox
            {
                ImageUrl = "https://lh3.googleusercontent.com/p0dsruAIgafKezYzhAU8qoRTUkrI8dAllAK2zucOaWt-XeJ0-1FTR4IHryZm1GSYWYF8yalPipmlI08Ju4sL--mT-Redwky7-t4S3ITo8QqsNXKL5Bma",
                BoundsF = new RectangleF(20, 15, 120, 40),
                Sizing = ImageSizeMode.StretchImage
            };
            detailBand.Controls.Add(logoMinas);
        }

        private void AdicionarTitulo(DetailBand detailBand)
        {
            XRLabel titleLabel = new XRLabel
            {
                Text = "Ingresso Reserva",
                Font = new Font("Work Sans", 16F, FontStyle.Bold),
                BoundsF = new RectangleF(205, 10, 400, 30),
                BorderColor = Color.AliceBlue
            };
            detailBand.Controls.Add(titleLabel);
        }

        //private void AdicionarCodigoQR(DetailBand detailBand)
        //{
        //    var qrCode = new XRBarCode
        //    {
        //        Symbology = new QRCodeGenerator(),
        //        AutoModule = true,
        //        BoundsF = new RectangleF(450, 50, 100, 100)
        //    };
        //    detailBand.Controls.Add(qrCode);
        //}
        private void AdicionarCodigoQR(DetailBand detailBand)
        {

            var qrCode = new XRPictureBox
            {
                Image = Image.FromFile(@"C:\Users\estagio.analise\Downloads\frame (1).png"),
                BoundsF = new RectangleF(450, 48, 100, 100),
                Sizing = ImageSizeMode.StretchImage
            };
            detailBand.Controls.Add(qrCode);
        }
        

        private void AdicionarCamposDeTexto(DetailBand detailBand)
        {
            float yOffset = 70;

            AddIngressoLabel(detailBand, "Nome:", "[Associado]", 20, yOffset, false, 42);
            AddIngressoLabel(detailBand, "Data da Reserva:", "[DataReservaFormatada]", 20, yOffset += 30, true, 110);
            AddIngressoLabel(detailBand, "NPF:", "[Npf]", 20, yOffset += 30, false, 30);
            AddIngressoLabel(detailBand, "Churrasqueira:", "[Churrasqueira.Nome]", 20, yOffset += 30, true, 97);
            AddIngressoLabel(detailBand, "Quantidade de Pessoas:", "[QtdPessoas]", 20, yOffset += 30, true, 160);
            AddDataGeracaoLabel(detailBand, 20, 220);
        }

        #endregion

        #region Métodos De Criação De Textos
        private void AddIngressoLabel(DetailBand band, string labelText, string expression, float x, float y, bool singleLine = false, float labelWidth = 150)
        {
            XRLabel labelTitle = new XRLabel
            {
                Text = labelText,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BoundsF = new RectangleF(x, y, singleLine ? 250 : 150, 20)
            };
            band.Controls.Add(labelTitle);

            XRLabel labelValue = new XRLabel
            {
                ExpressionBindings = { new ExpressionBinding("BeforePrint", "Text", expression) },
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BoundsF = new RectangleF(x + labelWidth + 5, y, 300, 20)
            };
            band.Controls.Add(labelValue);
        }
        #endregion

        #region Método Criar Data de Geração
        private void AddDataGeracaoLabel(DetailBand band, float x, float y)
        {
            CalculatedField dataGeracaoField = new CalculatedField
            {
                Name = "DataGeracao",
                Expression = "Now()"
            };
            this.CalculatedFields.Add(dataGeracaoField);

            XRLabel labelTitle = new XRLabel
            {
                Text = "Data da Geração:",
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BoundsF = new RectangleF(20, y, 150, 20)
            };
            band.Controls.Add(labelTitle);

            XRLabel labelValue = new XRLabel
            {
                ExpressionBindings = { new ExpressionBinding("BeforePrint", "Text", "[DataGeracao]") },
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BoundsF = new RectangleF(x + 117, y, 200, 20),
                TextFormatString = "{0:dd/MM/yyyy HH:mm:ss}"
            };
            band.Controls.Add(labelValue);
        }
        #endregion

        #region Método BackGround Imagem
        private void AdicionarImagemDeFundo(DetailBand detailBand)
        {
            XRPanel transparentOverlay = new XRPanel
            {
                BoundsF = new RectangleF(8, 8, 577, 257), // Certifique-se que os limites sejam iguais ao da imagem de fundo
                BackColor = Color.FromArgb(230, 255, 255, 255), // Ajuste a opacidade para 70%
                BorderWidth = 0
            };
            detailBand.Controls.Add(transparentOverlay);

            XRPictureBox backgroundImage = new XRPictureBox
            {
                Image = Image.FromFile(@"C:\Users\estagio.analise\Downloads\Minas-Tenis-Clube.jpg"),
                BoundsF = new RectangleF(8, 8, 577, 257),
                Sizing = ImageSizeMode.StretchImage
            };
            detailBand.Controls.Add(backgroundImage);

            backgroundImage.SendToBack();
        }
        #endregion

        private void InitializeComponent()
        {
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Name = "detailBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // IngressoReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.Version = "24.1";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}
