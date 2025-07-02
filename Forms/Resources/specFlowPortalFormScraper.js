/*
!! Switch the frame context to gsft_main before executing this script
*/

/**
 * Scrapes a portal form for namemap elements and gives suggestions for test steps for each field
 */
var specFlowPortalFormScraper = function () {
    var stepResults = jQuery("<textarea class='stepResults' style='width: 100%'></textarea>");

    var testStepTemplates = {};
    testStepTemplates.valuePlaceHolder = '[value]';
    testStepTemplates.identifierPlaceHolder = '[name]';
    testStepTemplates.TextBox = "Then I enter '[value]' in the '[name]' text field";
    testStepTemplates.Select = "Then I select the '[value]' option in the '[name]' list";
    testStepTemplates.Checkbox = "Then I check the '[name]' check box field";
    testStepTemplates.Radio = "Then I select the '[value]' radio option from the '[name]' field";
    testStepTemplates.Lookup = "Then I enter '[value]' in the '[name]' lookup text field";
    //#	#datetime
    //#	And I enter a date {1 } days from now in the 'dateTimePrettyName' date time field
    testStepTemplates.FirstStep = "Given I am on the '[name]' Form at '[value]'";
    testStepTemplates.LastSteps = "When I click the form submit button and wait for the page to finish loading\nThen I should see the title 'Thank you, your request has been submitted'\nAnd I should observe no script errors on the page";

    /**
     * String replaces the template with name and value
     * @param {any} template
     * @param {any} prettyName
     * @param {any} value
     */
    function writeTestStep(template, prettyName, value) {
        template = template.replace(testStepTemplates.valuePlaceHolder, value);
        template = template.replace(testStepTemplates.identifierPlaceHolder, prettyName);
        addStep(template);

        console.log(template);

        return template;
    }

    function addStep(step) {
        stepResults.val(stepResults.val() + "\n" + step);
    }

    var scrapeForm = function () {
        //get the namemap from the top level down
        var nameMap = window.top.frames['gsft_main'].g_form.nameMap

        //clean up from previous runs
        jQuery('.testStep').remove();
        jQuery('.stepResults').remove();

        //insert a textarea for our results
        jQuery(".rttms-catalog-item").before(stepResults)

        //add first steps
        writeTestStep(testStepTemplates.FirstStep, "FormName", window.top.location.href.replace(window.top.location.origin + "/", ""));

        //iterate the name map
        for (var i = 0; i < nameMap.length; i++) {
            var step = "";
            //filter only visible
            var input = jQuery(":input[name*='" + nameMap[i].realName + "']:visible");

            //iterate the jquery results
            for (var j = 0; j < input.length; j++) {
                var thisInput = jQuery(input[j]);

                //preconditions
                if (!jQuery(thisInput).is(":visible")) continue;
                if (thisInput.attr("id") === 'undefined') continue;

                if (thisInput.is("select")) {
                    step = writeTestStep(testStepTemplates.Select, nameMap[i].prettyName, thisInput.find(":selected").text());
                }
                else if (thisInput.is("textarea")) {
                    step = writeTestStep(testStepTemplates.TextBox, nameMap[i].prettyName, thisInput.val());
                }
                else if (thisInput.is("input")) {
                    var type = thisInput.attr("type");
                    if (!type)
                        type = 'text';//assume text

                    if (type == "radio")
                        step = writeTestStep(testStepTemplates.Radio, nameMap[i].prettyName, thisInput.val());
                    else if (type == "checkbox")
                        step = writeTestStep(testStepTemplates.Checkbox, nameMap[i].prettyName, "");
                    else if (type == "text") {
                        if (thisInput.attr("aria-autocomplete") == "list")
                            step = writeTestStep(testStepTemplates.Lookup, nameMap[i].prettyName, thisInput.val());
                        else
                            step = writeTestStep(testStepTemplates.TextBox, nameMap[i].prettyName, thisInput.val());
                    }
                    else continue;
                } else continue;

                //put the step text next to the input
                thisInput.after(jQuery("<div class='testStep' style='background: #a8d38d;'>" + step + "</div>"));
            }
        }
        //add last steps
        addStep(testStepTemplates.LastSteps);
    }
    scrapeForm();
}

new specFlowPortalFormScraper();