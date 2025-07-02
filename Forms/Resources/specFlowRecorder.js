
/**
 * Only works in the portal
 * execute in top frame
 */
function SpecFlowRecorder() {

    this.interactions = new Array();

    var debug = true;
    var self = this;

    var recording = false;
    var rec_timer;
    var rec_started = false;
    var rec_interval = 125;
    var rec_elapsed = 0;
    var form = window.frames['gsft_main'];

    this.testStepTemplates = {
        valuePlaceHolder: '[value]',
        identifierPlaceHolder: '[name]',
        TextBox: "Then I enter '[value]' in the '[name]' text field",
        Select: "Then I select the '[value]' option in the '[name]' list",
        Checkbox: "Then I check the '[name]' check box field",
        Radio: "Then I select the '[value]' radio option from the '[name]' field",
        Lookup: "Then I enter '[value]' in the '[name]' lookup text field",
        FirstStep: "Given I am on the '[name]' Form at '[value]'",
        LastSteps: "When I click the form submit button and wait for the page to finish loading\nThen I should see the title 'Thank you, your request has been submitted'\nAnd I should observe no script errors on the page"
    }


    this.frameCss = `
<style>
        .interacting {
            border: 1px solid #f0ad4e !important;
            background-color: lightyellow !important;
        }

        .interacted {
            border: 1px solid green !important;
            background-color: lightgreen !important;
        }
</style>
`;
    this.css =
`
 <style>
         .recFrame {
             pointer-events: none;
             border: 5px solid #999;
             width: 80px;
             height: 80px;
             position: fixed;
             z-index: 222;
             opacity: 0;
         }

        .recording {
            -webkit-animation: flickerAnimation 1s infinite;
        }

        .recFrame.tr {
            top: 20px;
            right: 20px;
            border-bottom: none;
            border-left: none;
        }

        .recFrame.tl {
            top: 20px;
            left: 20px;
            border-bottom: none;
            border-right: none;
        }

        .recFrame.br {
            bottom: 20px;
            right: 20px;
            border-top: none;
            border-left: none;
        }

        .recFrame.bl {
            bottom: 20px;
            left: 20px;
            border-top: none;
            border-right: none;
        }

        .recControls {
            position: fixed;
            z-index: 222;
            background: #ccc;
            padding: 10px;
            top: 30px;
            right: 30px;
            box-shadow: 0px 5px 17px 0px rgba(0,0,0,0.75);
        }

        .recControls * {
            display: block;
            line-height: 24px;
            float: left;
            margin-right: 10px;
        }

        .recControls .icon {
            background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAMAAAADACAQAAAD41aSMAAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAACYktHRAD/h4/MvwAAAAlwSFlzAAALEwAACxMBAJqcGAAAATBJREFUeNrt1GEKABAMgFGTKzs/Z5C0rPf917IXMZsy61YAAIAAABAAAAIAQAAACAAAAQAgAAAEAIAAABAAAAIAQAAACAAAAQAgAAAEAIAAABAAADpvPJ+wLs/H5/O9AF+QAAAQAAACAEAAAAgAAAEAIAAAAFgBAAACAEAAAAgAAAEAIAAABACAAAAQAAACAEAAAAgAAAEAIAAABACAAAAQAAACAEAAAAgAAAEAIAAABACAAAAQAAACAEAAAAgAAAEAIAAABACAAAAAIAAABACAAAAQAAACAEAAAAgAAAEAIAAABACAAAAQAAACAEAAAAhAhcbzCZF8w/ACBACAAAAQAAACAEAAAAgAAAEAAMAKAAAQAAACAEAAAAgAAAEAIAAABACAAAAQAAACAEAAyrUBe28D+/DKukEAAAAASUVORK5CYII=);
            background-repeat: no-repeat;
            background-size: 24px 24px;
            width: 24px;
            height: 24px;
        }

        .recControls .icon.active {
            background-image: url(data:image/png;base64,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);
        }

        .rec.active {
            -webkit-animation: flickerAnimation 1s infinite;
        }

        #recControl {
            width: 60px;
            text-decoration: underline;
            color: #e60d2e;
            cursor: pointer;
        }
        #recSw {
            width: 80px;
        }
        #rcResults{
            min-height: 200px;
            width: 100%;
            margin-top: 10px;
            display: none;
            margin-bottom: -10px;
        }
        .recInteractions {
            position: fixed;
            z-index: 222;
            background: #ccc;
            padding: 5px 0;
            top: 75px;
            right: 30px;
            min-width: 214px;
            text-align: center;
            font-size: 14px;
            box-shadow: 0px 5px 17px 0px rgba(0,0,0,0.75);
        }

        .interactionData {
            margin: 5px 0;
        }

        .interactionCount {
            font-weight: bold;
            color: #e60d2e;
        }



        .fixed_headers {
            width: 800px;
            table-layout: fixed;
            border-collapse: collapse;
            margin-bottom: 10px;
        }

            .fixed_headers th {
                text-decoration: underline;
            }

            .fixed_headers th,
            .fixed_headers td {
                padding: 5px;
                text-align: left;
            }

                .fixed_headers td:nth-child(1),
                .fixed_headers th:nth-child(1) {
                    min-width: 50px;
                }
                
                .fixed_headers td:nth-child(2),
                .fixed_headers th:nth-child(2) {
                    min-width: 50px;
                }
                .fixed_headers td:nth-child(3),
                .fixed_headers th:nth-child(3) {
                    width: 100px;
                }

                .fixed_headers td:nth-child(4),
                .fixed_headers th:nth-child(4) {
                    min-width: 250px;
                }

                .fixed_headers td:nth-child(5),
                .fixed_headers th:nth-child(5) {
                    width: 100px;
                }

                .fixed_headers td:nth-child(6),
                .fixed_headers th:nth-child(6) {
                    min-width: 250px;
                }



            .fixed_headers thead {
                background-color: #e60d2e;
                color: #fdfdfd;
            }

                .fixed_headers thead tr {
                    display: block;
                    position: relative;
                }

            .fixed_headers tbody {
                display: block;
                overflow: auto;
                width: 100%;
                height: 300px;
            }

                .fixed_headers tbody tr:nth-child(even) {
                    background-color: #efefef;
                }

        @keyframes flickerAnimation {
            0% {
                opacity: 1;
            }

            50% {
                opacity: 0;
                opacity: 0;
            }

            100% {
                opacity: 1;
            }
        }
    </style>
`;
    this.html =
`
    <div class="recFrame tr rec "></div>
    <div class="recFrame tl rec "></div>
    <div class="recFrame br rec "></div>
    <div class="recFrame bl rec "></div>
    <div class="recControls">
        <i class="icon rec "></i>
        <a id="recControl" onclick="top.recorder.toggleRecord()" >Record</a>
        <span id="recSw">00:00:00</span>
    </div>
    <div class="recInteractions">
        <span class="interactionCount">0</span> Interations recorded <a onclick="top.recorder.toggleInteractions()" href="javascript:void(0)" class="toggleInteraction">Show</a>
        <div id="interactionData" style="display: none;">
            <table class="fixed_headers interactionTable">
                <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>Order</th>                        
                        <th>Time</th>
                        <th>Field</th>
                        <th>Type</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody id="rcInteractionTableBody"></tbody>
            </table>
            <div class="interactionControls">
                <button onclick="top.recorder.reset()" class="mdl-button mdl-button--raised">Discard and Reset</button>
                <button onclick="top.recorder.generateSteps()" class="mdl-button mdl-button--raised rtp_button-primary">Generate Test Steps</button>
            </div>
            <div class="testSteps">
                  <textarea id="rcResults"></textarea>
            </div>
        </div>

    </div>
`;


    /**
     * Interogates the g_form.nameMap and Initialises input listeners
     */
    this.init = function () {
        form = window.frames['gsft_main'];
        //inject the html and css into the body on the top level and in the frame
        form.jQuery("body").append(jQuery(this.frameCss));
        top.jQuery("body").append(jQuery(this.css));
        top.jQuery("body").append(jQuery(this.html));

        var nameMap = window.top.frames['gsft_main'].g_form.nameMap;
        //iterate the name map and get the corrosponding inputs
        for (var i = 0; i < nameMap.length; i++) {

            //create a new property on the nameMap to link the entry with an input(s)
            var nameMapEntry = nameMap[i];
            nameMapEntry.inputs = new Array();
            nameMapEntry.guid = nameMapEntry.realName.replace("IO:", "");
            //filter only inputs
            var inputQuery = form.jQuery(":input[id*='" + nameMapEntry.guid + "']");

            //iterate the jquery results
            for (var j = 0; j < inputQuery.length; j++) {
                var thisInput = form.jQuery(inputQuery[j]);

                var thisInputType = thisInput.prop("type");

                //===check preconditions
                //Only elements with an Id
                if (thisInput.attr("id") === 'undefined') continue;
                if (thisInputType !== 'text'
                    && thisInputType !== 'checkbox'
                    && thisInputType !== 'textarea'
                    && thisInputType !== 'select-one'
                    && thisInputType !== 'radio') continue;

                //record some info and store as props on the input
                thisInput.prop("data-prettyName", nameMapEntry.prettyName);
                thisInput.prop("data-initialValue", getInputValue(thisInput));
                thisInput.prop("data-lastValue", getInputValue(thisInput));
                thisInput.prop("data-hasInteracted", false);

                //hook up interaction events to the input
                thisInput.off("focus", onInteraction);
                thisInput.on("focus", onInteraction);

            }

        }

    }

    this.reset = function () {

        if (recording)
            self.toggleRecord();
        if (rec_timer)
            clearTimeout(rec_timer);
        rec_elapsed = 0;
        top.jQuery("#recSw").text(msToTime(0));
        self.interactions = new Array();
        self.toggleInteractions();
        refreshInteractions();
        hideResults();
    }



    /**
     * Starts/Stops interaction recording
     * @returns {} 
     */
    this.toggleRecord = function () {

        //toggle recording flag
        recording = !recording;

        if (recording) {
            //update display
            top.jQuery(".rec").addClass("active");
            top.jQuery("#recControl").text("Pause");
            //start timer
            rec_timer = setInterval(onRecTimer, rec_interval);
        } else {
            //update display
            top.jQuery(".rec").removeClass("active");
            top.jQuery("#recControl").text("Record");
            //clear timer
            clearTimeout(rec_timer);
        }
    }

    /**
     * Toggles visibility of the Interactions table
     * @returns {} 
     */
    this.toggleInteractions = function () {
        if (top.jQuery("#interactionData").is(":visible")) {
            top.jQuery("#interactionData").hide();
            top.jQuery(".toggleInteraction").text("Show");
        } else {
            top.jQuery("#interactionData").show();
            top.jQuery(".toggleInteraction").text("Hide");
        }
    }

    /**
     * Generates specflow steps from interactions
     * @returns {} 
     */
    this.generateSteps = function () {

        //stop recording if we are still recording
        if (recording)
            self.toggleRecord();

        var stepText = "";

        //write the first steps
        stepText += writeTestStep(self.testStepTemplates.FirstStep, top.frames["gsft_main"].cat_item.name, window.top.location.href.replace(window.top.location.origin + "/", ""));;

        //iterate the interactions and write form control steps
        for (var i = 0; i < self.interactions.length; i++) {
            var interaction = self.interactions[i];
            var template = null;

            switch (interaction.type) {
                case "radio":
                    template = self.testStepTemplates.Radio;
                    break;
                case "select-one":
                    template = self.testStepTemplates.Select;
                    break;
                case "checkbox":
                    template = self.testStepTemplates.Checkbox;
                    break;
                default:
                    template = self.testStepTemplates.TextBox;
            }
            stepText += writeTestStep(template, interaction.name, interaction.newValue);
        }

        //write last steps
        stepText += writeTestStep(self.testStepTemplates.LastSteps);

        //put the steps in the textarea
        showResults(stepText);
        
        info("Test steps have been generated \n=====================================\n\n" + stepText, "SpecFlow Recorder");
    }

    /**
 * Discards an interaction
 * @param {} index 
 * @returns {} 
 */
    this.discardInteraction = function (index) {
        if (confirm("Discard this interaction?")) {
            self.interactions.splice(index, 1);
            refreshInteractions();
        }
    }

    var showResults = function(results) {
        top.jQuery("#rcResults").val(results);
        top.jQuery("#rcResults").focus();
        top.jQuery("#rcResults").select();
        top.jQuery("#rcResults").show();
    }

    var hideResults = function() {
        top.jQuery("#rcResults").val("");
        top.jQuery("#rcResults").hide();
    }

    /**
     * String replaces the template with name and value
     * @param {any} template
     * @param {any} prettyName
     * @param {any} value
     */
    var writeTestStep = function(template, prettyName, value) {
        template = template.replace(self.testStepTemplates.valuePlaceHolder, value);
        template = template.replace(self.testStepTemplates.identifierPlaceHolder, prettyName);
        info(template);

        return template + "\n";
    }

    /**
     * Refreshes dispaly of interactions
     */
    var refreshInteractions = function () {
        //refresh count
        top.jQuery(".interactionCount").text(self.interactions.length);
        //clear the table contents
        top.jQuery("#rcInteractionTableBody").html("");

        //add table items
        for (var i = 0; i < self.interactions.length; i++) {
            var interaction = self.interactions[i];
            var interactionNewValue = interaction.newValue.toString().substring(0, 25) + (interaction.newValue.toString().length >= 25 ? "..." : "");
            var row = jQuery(`
            <tr>
                <td><a href="javascript:void(0)" onclick="recorder.discardInteraction(` + i +`)">discard</a></td>
                <td>` + i + `</td>
                <td>` + msToTime(interaction.elapsed) + `</td>
                <td>` + interaction.name + `</td>
                <td>` + interaction.type + `</td>
                <td>` + interactionNewValue + `</td>
            </tr>`);
            top.jQuery("#rcInteractionTableBody").prepend(row);
        }
    };

    var onInteraction = function (ev) {
        //check preconditions
        if (!recording) return;

        var input = form.jQuery(ev.target);
        info(input.prop("data-prettyName") + " received focus");

        //visuals
        input.addClass("interacting");

        //we are expecting a change to happen here 
        //so we hook up the blur event and check the value when that fires
        //but first make sure we dont duplicate handlers
        input.off("blur", onInteractionEnded);
        input.on("blur", onInteractionEnded);

    }

    var onInteractionEnded = function (ev) {

        var input = form.jQuery(ev.target);
        info(input.prop("data-prettyName") + " lost focus");


        //unhook the event. This is a one time call
        input.off("blur", onInteractionEnded);

        //get the current value and initial value
        var currentVal = getInputValue(input);
        var initialVal = input.prop("data-initialValue");
        var lastVal = input.prop("data-lastValue");
        var inputName = input.prop("data-prettyName");


        if (currentVal !== lastVal) {

            //record the interaction
            self.interactions.push({
                input: input,
                name: inputName,
                oldValue: lastVal,
                newValue: currentVal,
                type: input.prop("type"),
                elapsed: rec_elapsed
            });

            //record the change on the input
            setLastInputValue(input, currentVal);

            //record that the input has been touched
            input.prop("data-hasInteracted", true);

            //form element visuals
            input.addClass("interacted");

            //interaction table
            refreshInteractions();

            info("Interaction ended (" + input.prop("data-prettyName") + ") => Initial: [" + initialVal + "], Last: [" + lastVal + "], Current: [" + currentVal + "]");
            info("Recorded " + self.interactions.length + " interactions");
            info(self.interactions);


        }
        else {
            info("NoCHANGE Interaction ended (" + input.prop("data-prettyName") + ") => Initial: [" + initialVal + "], Last: [" + lastVal + "], Current: [" + currentVal + "]");
        }

        input.removeClass("interacting");
    }




    /**
     * timer handler
     */
    var onRecTimer = function () {
        rec_elapsed += rec_interval;
        top.jQuery("#recSw").text(msToTime(rec_elapsed));
    }

    /**
     * gets the input value
     * @param {any} input
     */
    var getInputValue = function (input) {
        var inputType = input.prop("type");

        if (inputType === 'checkbox')
            return input.prop("checked");
        if (inputType === 'select-one')
            return input.find("option:selected").text();
        if (inputType === 'radio')
            return getSelectedRadioText(input);
        return input.val();
    }

    var setLastInputValue = function (input, value) {

        if (input.prop("type") === 'radio') {
            //set value on all elements in the group
            var options = form.jQuery("input[name='" + input.prop("name") + "']");
            options.prop("data-lastValue", value);
        } else {

            input.prop("data-lastValue", value);
        }
    }

    var getSelectedRadioText = function (input) {
        var selectedRadioId = form.jQuery("input[name='" + input.prop("name") + "']:checked").prop("id");
        return form.jQuery("label[for='" + selectedRadioId + "']").text();
    }

    var info = function (message) {
        if (debug === true)
            console.info(message);
    }


    /**
     * Converts ms to a readable format
     * @param {any} duration
     */
    var msToTime = function (duration) {
        var seconds = parseInt((duration / 1000) % 60), minutes = parseInt((duration / (1000 * 60)) % 60);
        minutes = (minutes < 10) ? "0" + minutes : minutes;
        seconds = (seconds < 10) ? "0" + seconds : seconds;
        return minutes + ":" + seconds + "." + ("000" + duration % 1000).slice(-3);
    }


    this.init();
}

var recorder = new SpecFlowRecorder();

