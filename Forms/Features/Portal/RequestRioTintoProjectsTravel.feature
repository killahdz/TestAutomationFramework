Feature: Forms.Features.Portal.RequestRioTintoProjectsTravel 
	As a portal user
	I want to know I can successfully 
	submit the project travel form

#@dev @patch @uat @video
Scenario: RequestRioTintoProjectsTravel
	Given I am on the 'Request Rio Tinto Projects Travel' Form at 'rttms/#/order/item/RT_Projects_Travel'	
	Then I dismiss notifications
	Then I check the 'urgent_travel_request' check box field
	Then I enter 'Mr Bob Dobalina' in the 'requesting_for_passport_name' text field		
	Then I enter '0749255511' in the 'requesting_for_phone' text field	
	Then I select the 'Paraburdoo Mine Site' option in the 'project_location' list
	Then In the 'grid_emergency_contacts' grid I clear the existing rows
	Then In the 'grid_emergency_contacts' grid I create a new row and enter the values 'Paddington Bear;071111111;041111111;cb:Personal'
	Then In the 'grid_emergency_contacts' grid I create a new row and enter the values 'Bill Lumbergh;071111111;041111111;cb:Business'	
	Then I enter '101' random characters into the 'justification' text field
	Then I check the 'approval_confirmation_check' check box field
	Then I select the 'International' radio option from the 'trip_type' field
	Then I select the 'Return' radio option from the 'trip_segments' field	
	Then I enter 'Brisbane, Albert Street' in the 'trip_origin' autocomplete field and select the first item
	Then I enter 'Auckland International Airport' in the 'trip_destination' autocomplete field and select the first item
	Then I select the 'Arriving as late as possible before' option in the 'outward_date_requirement' list
	Then I select the 'Arriving as late as possible before' option in the 'return_date_requirement' list	
	Then I enter a date {1} days from now in the 'outward_date_time' date time field
	Then I enter a date {2} days from now in the 'return_date_time' date time field
	Then I check the 'additional_approval_check' check box field				
	Then I select the 'Yes' radio option from the 'approvals_required' field
	Then In the 'approvers_grid' grid I clear the existing rows
	Then In the 'approvers_grid' grid I create a new row and enter the values 'li:Managing Director (MD);cb:Brassard, Isabelle' 
	Then I select the 'Australia Dollars | AUD$' option in the 'currency_of_cost' list 
	Then I enter '2314.00' in the 'estimated_total_cost' text field	
	Then In the 'grid_cost_centre' grid I enter the values ';;li:Cost centre;cb:10000' 	
	Then I enter 'This is a nice form' in the 'comments' text field	
	Then I add an attachment
	When I click the form submit button and wait for the page to finish loading	
	Then I should see the title 'Thank you, your request has been submitted' 