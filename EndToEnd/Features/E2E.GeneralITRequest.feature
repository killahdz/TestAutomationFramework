Feature: EndToEnd.Features.GeneralITRequest
	As a Resolver
	I want to be able to submit a general IT Request on someones behalf
	then log in as a resolver and complete the request

@dev @video
Scenario: E2E.GeneralITRequest
	#1. Login as testing user
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Given I go to this url 'rttms/#/order/search' and wait until I see the element with this class 'mdl-list'
	Then I dismiss notifications
	
	#request for Daniel Kereama
	When I request for 'Daniel Kereama'
	Then I should see 'Kereama, Daniel (IST-PIPEFISHPL)' as the active requestor
	Then I wait for 5 seconds

	#complete the general IT Request form
	Given I am on the 'General IT Request Form' Form at 'rttms/#/order/item/General_Incident'
	Then I dismiss notifications
	Then I enter 'AUBNEWL00000' in the 'computerName' text field
	And I enter 'I need help!!' in the 'IT_request_desc' text field
	Then I add an attachment
	When I click the form submit button and wait for the page to finish loading	
	Then I should see the title 'Thank you, your request has been submitted' 

	#record the ticket number before going into resolver view
	Then I remember my Unique Ticket Number

	#go to resolver view
	Given I am on the home page
	Then I impersonate as 'Susan Halloran'
	Then I go to the 'Tickets' List at 'incident_list.do'
	Then I search the Resolver list 'incident' by 'Number' for my Unique Ticket Number
	Then I open the first list item

	#verify some of the fields on the form	
	Then I verify the text 'AUBNEWL00000' exists in the 'incident.description' resolver text field
	Then I verify the text 'I need help!!' exists in the 'incident.description' resolver text field
	Then I dismiss notifications
	Then I verify that an attachment exists on the resolver form

	#fill in the required fields
	Then I enter the text 'Email' into the 'sys_display.incident.u_business_service' resolver suggestion text field
	Then I select the 'Software' option in the 'incident.category' resolver select field
	Then I select the 'Access' option in the 'incident.subcategory' resolver select field
	Then I enter the text 'RTTMS' into the 'sys_display.incident.cmdb_ci' resolver suggestion text field
	Then I enter the text 'Guevarra, Justin' into the 'sys_display.incident.assigned_to' resolver suggestion text field

	#Go to the Closeure details tab and complete the required fields
	Then I click the 'Closure Details' tab
	Then I select the 'Solved' option in the 'incident.u_resolution_code' resolver select field
	Then I enter the text 'Fixed it! You owe me...' into the 'incident.u_solution' resolver text field
	Then I click the Resolve Ticket button and check for form submission errors
	