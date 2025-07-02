Feature: Forms.Features.Portal.RequestGeneralITAssistance 
	As a portal user
	I want to know I can successfully 
	submit the General IT request form

@dev @patch @uat @video
Scenario: RequestGeneralITAssistance
	Given I am on the 'General IT Request Form' Form at 'rttms/navpage.do#/order/item/General_Incident'
	Then I dismiss notifications
	Then I enter 'AUBNEWL00000' in the 'computerName' text field
	And I enter 'I need help!!' in the 'IT_request_desc' text field
	Then I add an attachment
	When I click the form submit button and wait for the page to finish loading	
	Then I should see the title 'Thank you, your request has been submitted' 

