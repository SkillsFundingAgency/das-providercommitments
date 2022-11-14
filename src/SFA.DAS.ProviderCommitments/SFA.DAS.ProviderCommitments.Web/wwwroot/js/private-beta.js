function continueJourney() {
    validateDraftApprenticeship(showNextPage);
}

function showNextPage() {
    $("#draftApprenticeshipSection1").addClass("das-hide");
    $("#draftApprenticeshipSection2").removeClass("das-js-hide");
    $("#draftApprenticeshipSection2").addClass("das-show");
    $("#submitAddDraftApprenticeship").removeClass("das-js-hide");
    $("#submitAddDraftApprenticeship").addClass("das-show");

    if ($("#IsOnFlexiPaymentPilot").prop("checked") === true) {
        ensureActualStartDatePopulation();
        $("#start-date-section").addClass("das-hide");
    } else {
        ensureStartDatePopulation();
        $("#actual-start-date-section").addClass("das-hide");
    }

    $("#LastName").closest("form").on('submit', handleSubmit);
    window.scrollTo(0, 0);
}

function ensureActualStartDatePopulation() {

    var actualStartMonth = $("input#ActualStartMonth").val();
    if (actualStartMonth != null && actualStartMonth.trim() !== '')
        return;

    var month = $("input#StartMonth").val();
    var year = $("input#StartYear").val();

    $("input#ActualStartDay").val("");
    $("input#ActualStartMonth").val(month);
    $("input#ActualStartYear").val(year);

    $("input#StartMonth").val("");
    $("input#StartYear").val("");
}

function ensureStartDatePopulation() {

    var startMonth = $("input#StartMonth").val();
    if (startMonth != null && startMonth.trim() !== '')
        return;

    var month = $("input#ActualStartMonth").val();
    var year = $("input#ActualStartYear").val();

    $("input#StartMonth").val(month);
    $("input#StartYear").val(year);

    $("input#ActualStartDay").val("");
    $("input#ActualStartMonth").val("");
    $("input#ActualStartYear").val("");
}

function resetErrors() {
    $("#validationSummaryErrorList").empty();
    $("#validationSummary").removeClass("das-show");
    $("#validationSummary").addClass("das-hide");
    $("*").removeClass("govuk-form-group--error");
}

function showError(errorDetail) {
    var fieldId = errorDetail.field;
    $("#validationSummary").removeClass("das-hide");
    $("#validationSummary").addClass("das-show");
    $("#validationSummaryErrorList").append("<li><a href=\"#error-message-" + fieldId + "\" data-focuses=\"error-message-" + fieldId + "\">" + errorDetail.message + "</a></li>");
    $("#" + fieldId).closest(".govuk-form-group").addClass("govuk-form-group--error");
    $("#" + fieldId).closest(".govuk-fieldset__legend").after("<span class=\"field-validation-error govuk-error-message\" data-valmsg-for=\"" + fieldId + "\" data-valmsg-replace=\"true\" id=\"error-message-" + fieldId + "\">" + errorDetail.message + "</span>");
}

function validateDraftApprenticeship(onSuccess) {
    resetErrors();

    var token = $('input[name="__RequestVerificationToken"]').val();

    var $form = $("#LastName").closest("form");
    var formData = getFormData($form);
    formData.__RequestVerificationToken = token;
    formData.providerId = window.urls.providerId;
    $.ajax({
        url: window.urls.validate,
        method: "POST",
        data: formData,
        context: $form,
        success: function (data) {
            if (!data) {
                onSuccess($form);
                return;
            }
            $("#submitAddDraftApprenticeship").prop("disabled", false);
            data.forEach(function (errorDetail) {
                showError(errorDetail);
            });
        },
        error: function (data) {
            console.log(data);
        }
    });

    return false;
}

function handleSubmit(e) {
    var submitterId = e.originalEvent.submitter.id;
    if (submitterId === "change-course-link" || submitterId === "change-delivery-model-link") return;
    e.preventDefault();
    e.returnValue = false;

    validateDraftApprenticeship(submitForm);
}

function submitForm($form) {
    $form.off('submit');
    $form.submit();
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array,
        function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

    return indexed_array;
}

document.addEventListener("DOMContentLoaded", showTrainingDetailsIfRequired);

function showTrainingDetailsIfRequired() {
    var urlParams = new URLSearchParams(window.location.search);
    var showTrainingDetails = urlParams.get("showTrainingDetails") ||
        previousPageWasEitherOf("select-course", "select-delivery-model");

    if (showTrainingDetails) {
        continueJourney();
    }
}

function previousPageWasEitherOf() {
    for (var i = 0; i < arguments.length; i++) {
        if (document.referrer.indexOf(arguments[i]) > -1) return true;
    }
    return false;
}