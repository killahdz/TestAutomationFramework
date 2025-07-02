Feature: Forms.Features.Portal.RequestTranslationService
		As a portal user
		I want to know I can successfully 
		submit the Request Translation Service form

#@dev @patch @uat @video
Scenario: RequestTranslationService	
	Given I am on the 'Request Translation Service' Form at 'rttms/#/order/item/Translation_Service'
	Then I select the 'Translate document' option in the 'request_type' list
	Then I enter 'Rio Tinto' in the 'IT_dmn_details_org' lookup text field
	Then I enter 'so short' in the 'translate_short_description' text field
	Then I enter 'help me' in the 'request_details' text field	
	Then I select the 'English' option in the 'language_from' list
	Then I select the 'French' option in the 'language_to' list		
	Then I enter a date {10} days from now in the 'date_to_translate_by' date time field with format 'yyyy-MM-dd'	
	When I click the form submit button and wait for the page to finish loading
	Then I should see the title 'Thank you, your request has been submitted'
	And I should observe no script errors on the page
