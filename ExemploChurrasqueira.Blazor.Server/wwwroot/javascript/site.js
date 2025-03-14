function saveAsFile(filename, base64Data) {
    const link = document.createElement('a');
    link.href = 'data:text/json;base64,' + base64Data;
    link.download = filename;
    link.click();
};

window.addGlobalChangeEvent = () => {
    document.querySelectorAll('.parametrized-action-wrapper.xaf-action-fulltextsearch.dxbl-text-edit')
        .forEach(element => {
            element.addEventListener('change', (event) => {
                console.log('Valor alterado:', event.target.value);
            });
        });
};

