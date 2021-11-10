// Autocomplete on input fields, with data available on the page

var autocompleteInputs = document.querySelectorAll(".app-input-autocomplete");
if (autocompleteInputs.length > 0) {

    for (var i = 0; i < autocompleteInputs.length; i++) {

        var input = autocompleteInputs[i]
        var container = document.createElement('div');
        var apiUrl = input.dataset.autocompleteUrl;

        container.className = "das-autocomplete-wrap"
        input.parentNode.replaceChild(container, input);

        accessibleAutocomplete({
            element: container,
            id: input.id,
            name: input.name,
            defaultValue: input.value,
            displayMenu: 'overlay',
            showNoOptionsFound: false,
            minLength: 3,
            source: autoCompleteSource,
            placeholder: "",
            confirmOnBlur: false,
            autoselect: true
        });
    }
}


$(function () {

  // Autocomplete on dropdowns  

  var selectElements = $('.das-autocomplete')
    selectElements.each(function () {
        var form = $(this).closest('form');
        accessibleAutocomplete.enhanceSelectElement({
            selectElement: this,
            minLength: 3,
            autoselect: false,
            defaultValue: '',
            showAllValues: true,
            displayMenu: 'overlay',
            dropdownArrow: function () {
                return '<svg width="17" height="11" viewBox="0 0 17 11" xmlns="http://www.w3.org/2000/svg"><path d="M1.97 0L8.39 6.4L14.8 0L16.77 1.97L8.38 10.36L0 1.97L1.97 0Z" fill="black"/></svg>';
            },
            placeholder: $(this).data('placeholder') || '',
            onConfirm: function (opt) {
                var txtInput = document.querySelector('#' + this.id);
                var searchString = opt || txtInput.value;
                var requestedOption = [].filter.call(this.selectElement.options,
                    function (option) {
                        return (option.textContent || option.innerText) === searchString
                    }
                )[0];
                if (requestedOption) {
                    requestedOption.selected = true;
                } else {
                    this.selectElement.selectedIndex = 0;
                }
            }
        });
        form.on('submit', function() {
            $('.autocomplete__input').each(function() {
                var that = $(this);
                if (that.val().length === 0) {
                    var fieldId = that.attr('id'),
                    selectField = $('#' + fieldId + '-select');
                    selectField[0].selectedIndex = 0;
                }
            });
        });
    })

    // Length limit on fields 

    $(document).on('input', '.length-limit', function () {
        var text = $(this).val();
        var len = text.length;
        var maxlength = $(this).attr('maxlength');

        if (maxlength == null) return;

        if (len > maxlength) {
            $(this).val(text.substring(0, maxlength));
        }
    });
});