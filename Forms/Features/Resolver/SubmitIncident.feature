Feature: Forms.Features.Resolver.SubmitIncident
	As a resolver, I want to create a new incident in the resolver view

@dev @patch @uat @video
Scenario: SubmitIncident
 	Given I am on the home page
	Then I go to the 'Incident' Resolver Form at 'incident.do?sys_id=-1'	
	Then I dismiss notifications
	Then I enter the text 'Kereama, Daniel' into the 'sys_display.incident.caller_id' resolver suggestion text field
	Then I select the '1 - Major' option in the 'incident.impact' resolver select field
	Then I enter the text 'IST RTTMS - Development' into the 'sys_display.incident.assignment_group' resolver suggestion text field
	Then I enter the text 'Kereama, Daniel' into the 'sys_display.incident.assigned_to' resolver suggestion text field
	Then I enter the text 'Application Hosting' into the 'sys_display.incident.u_business_service' resolver suggestion text field
	Then I dismiss notifications
	Then I check the 'label.ni.incident.u_third_party_flag' label for the resolver checkbox field
	Then I enter the text 'Pipefish' into the 'sys_display.incident.u_third_party' resolver suggestion text field	
	Then I enter the text 'I need Help!!' into the 'incident.short_description' resolver text field
	Then I enter the text 'I need lots of Help!!' into the 'incident.description' resolver text field
	Then I click the Save button and wait for the form to reload
	Then I should observe no script errors on the page
	
