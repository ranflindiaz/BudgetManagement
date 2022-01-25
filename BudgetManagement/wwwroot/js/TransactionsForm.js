function inicializeTransactionsForm(urlGetCategories) {
    $("#OperationTypeId").change(async function () {
        const selectedValue = $(this).val();

        const response = await fetch(getCategoriesUrl, {
            method: 'POST',
            body: selectedValue,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await response.json();
        const options =
            json.map(categories => `<option value=${categories.value}>${categories.text}</option>`);
        $('#CategoryId').html(options);
    })
}