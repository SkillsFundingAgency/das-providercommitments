function ensureDatePopulationBasedOnPilotStatus() {
    if ($("#pilot-status-value").text().indexOf("Yes") >= 0) {
        ensureActualStartDatePopulation();
    } else {
        ensureStartDatePopulation();
    }
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

document.addEventListener("DOMContentLoaded",
    function () {
        console.log("DOMContentLoaded");
        ensureDatePopulationBasedOnPilotStatus();
    });