Feature: Forms.Features.Portal.RequestLaptop
		As a portal user
		I want to know I can successfully 
		submit the Request Laptop form

#@dev @patch @uat @video
Scenario: RequestLaptop
	Given I am on the 'FormName' Form at 'rttms/#/order/item/Laptop'
	Then I select the 'New' option in the 'laptop_order_type' list
	Then I select the 'High Performance' option in the 'laptop_type' list
	Then I check the 'refurbished' check box field
	Then I check the 'q_include_accessories' check box field
	Then I check the 'keyboard' check box field
	Then I check the 'mouse' check box field
	Then I check the 'mini_display' check box field
	Then I check the 'privacy_screen' check box field
	Then I check the 'monitor_stand' check box field
	Then I check the 'laptop_stand' check box field
	Then I check the 'docking_station' check box field
	Then I check the 'memory_upgrade' check box field
	Then I check the 'video_card' check box field
	Then I check the 'power_adapter' check box field
	Then I check the 'battery' check box field
	Then I check the 'backpack' check box field
	Then I check the 'solid_state_drive' check box field
	Then I select the 'Standard' option in the 'keyboard_type' list
	Then I select the 'Standard' option in the 'mouse_type' list
	Then I select the 'New' option in the 'video_card_type' list
	Then I enter '123, Albert Street, Brisbane 4000, QLD, Australia' in the 'delivery' text field
	Then I select the 'Operational' option in the 'funding_type' list
	Then I enter '0010501225' in the 'cost_centre' lookup text field	
	Then I enter 'I like new computers' in the 'business_justification' text field
	Then I enter 'Hurry ' in the 'comments' text field
	When I click the form submit button and wait for the page to finish loading
	Then I should see the title 'Thank you, your request has been submitted'
	And I should observe no script errors on the page
