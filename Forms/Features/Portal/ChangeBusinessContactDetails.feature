Feature: Forms.Features.Portal.ChangeBusinessContactDetails
		As a portal user
		I want to know I can successfully 
		submit the Change Business Contact Details form

#@dev @patch @uat @video
Scenario: ChangeBusinessContactDetails
	Given I am on the 'FormName' Form at 'rttms/#/order/item/Edit_User_Profile'
	Then I enter 'AUBNEWL00000' in the 'computerName' text field
	Then I enter '04111111111' in the 'phone' text field
	Then I enter '04111111111' in the 'mobile_phone' text field
	Then I enter '04111111111' in the 'u_fax_phone' text field
	Then I enter '04111111111' in the 'u_other_phone' text field
	Then I enter 'Mr T' in the 'u_assistant' text field
	Then I enter '04111111111' in the 'u_assistant_phone' text field
	Then I check the 'hse_travel_alert' check box field
	Then I enter 'Hello' in the 'comments' text field
	When I click the form submit button and wait for the page to finish loading
	Then I should see the title 'Thank you, your request has been submitted'
	And I should observe no script errors on the page