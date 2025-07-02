Feature: EndToEnd.Features.TravelRequest
	As a Resolver
	I want to be able to submit a Travel Request on someones behalf
	then log in as a resolver and complete the request

@dev @video
Scenario: E2E.TravelRequest
	#1. Login as testing user
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Given I go to this url 'rttms/#/order/search' and wait until I see the element with this class 'mdl-list'
	Then I dismiss notifications
	
	#2. request for Daniel Kereama
	When I request for 'Daniel Kereama'
	Then I should see 'Kereama, Daniel (IST-PIPEFISHPL)' as the active requestor
	
	#3. fill in the form
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

	#4. Remember ticket number
	Then I remember my Unique Ticket Number

	#5. Log into resolver view and impersonate a resolver
	Given I am on the home page
	Then I impersonate as 'Susan Halloran'

	#6. Search for our RITM and open it
	Then I go to the 'Tickets' List at 'sc_req_item_list.do'	
	Then I search the Resolver list 'sc_req_item' by 'Number' for my Unique Ticket Number
	Then I open the first list item

	#7. Validate the request details data we entered earlier
	Then I click the 'Request Details' tab
	Then I verify the 'urgent_travel_request' request details check box field is checked
	Then I verify 'Mr Bob Dobalina' exists in the 'requesting_for_passport_name' request details text field		
	Then I verify '0749255511' exists in the 'requesting_for_phone' request details text field		
	Then I verify the 'Paraburdoo Mine Site' option is selected in the 'project_location' request details list
	Then I Verify the 'grid_emergency_contacts' grid contains the row 'Paddington Bear;071111111;041111111;Personal'
	Then I Verify the 'grid_emergency_contacts' grid contains the row 'Bill Lumbergh;071111111;041111111;Business'	
	Then I verify the 'approval_confirmation_check' request details check box field is checked
	Then I verify the 'International' option is selected from the 'trip_type' request details radio field
	Then I verify the 'Return' option is selected from the 'trip_segments' request details radio field
	Then I verify 'Brisbane, Albert Street' exists in the 'trip_origin' request details autocomplete field		
	Then I verify 'Auckland International Airport' exists in the 'trip_destination' request details autocomplete field		
	Then I verify the 'Arriving as late as possible before' option is selected in the 'outward_date_requirement' request details list
	Then I verify the 'Arriving as late as possible before' option is selected in the 'return_date_requirement' request details list
	#these verify date steps need to be updated to pass in expected date format. currently they are dd-MM-yyyy hh:mm:ss in the resovler view
	#Then I verify the date is approximately {1} days from now in the 'outward_date_time' request details date time field
	#Then I verify the date is approximately {2} days from now in the 'return_date_time' request details date time field
	Then I verify the 'additional_approval_check' request details check box field is checked
	Then I verify the 'Yes' option is selected from the 'approvals_required' request details radio field
	Then I Verify the 'approvers_grid' grid contains the row 'Managing Director (MD);Brassard, Isabelle'
	Then I verify the 'Australia Dollars | AUD$' option is selected in the 'currency_of_cost' request details list
	Then I verify '2,314.00' exists in the 'estimated_total_cost' request details text field		
	Then I Verify the 'grid_cost_centre' grid contains the row 'Cost centre;10000000 DEFAULT CC CC1000'
	Then I verify 'This is a nice form' exists in the 'comments' request details text field		

	#8. Remember the current Url for our RITM
	Then I remember the current Url for the 'Requested Item' page
	
	#9. Complete the Seeking Quote Task
	Then I click the 'Catalog Tasks' tab
	Then I open the 'Seeking quote' Catalog Task
	Then I select the 'Closed Complete' option in the 'sc_task.state' resolver select field
	Then I enter the text 'Automated Tester was here' into the 'sc_task.description' resolver text field
	Then I click the 'Activity' tab
	Then I enter the text 'Quote obtained: $2,314.00 flying Qantas flight 2345' into the 'activity-stream-comments-textarea' resolver text field
	Then I click the Complete Task button and wait for the form to reload
	Then I return to the Url I remembered for the 'Requested Item' page

	#10. Complete the Seeking approval task
	Then I click the 'Catalog Tasks' tab
	Then I open the 'Seeking approval' Catalog Task
	Then I select the 'Closed Complete' option in the 'sc_task.state' resolver select field
	Then I click the 'Activity' tab
	Then I enter the text 'Quote approved by Automated Tester' into the 'activity-stream-comments-textarea' resolver text field
	Then I click the Request Approvals button and wait for the form to reload	
	Then I return to the Url I remembered for the 'Requested Item' page

	#11. Complete the Processing Booking task
	Then I click the 'Catalog Tasks' tab
	Then I open the 'Processing Booking' Catalog Task
	Then I select the 'Closed Complete' option in the 'sc_task.state' resolver select field
	Then I click the 'Activity' tab
	Then I enter the text 'Itinerary sent to customer' into the 'activity-stream-comments-textarea' resolver text field
	Then I click the 'Request Details' tab
	Then I set the option 'Yes' on the 'travel_booked' request details radio group
	Then I set the value 'Flight booked by Automated tester' on the 'customer_message' request details textbox
	Then I include the first Itenerary attachment
	Then I click the Complete Task button, accept alerts and wait for the form to reload
	
	#12. impersonate the original requestor
	Given I am on the home page
	Then I impersonate as 'Daniel Kereama'

	#13. Search for our completed ticket in the portal
	Given I go to this url 'rttms/#/track/search/all' and wait until I see the element with this class 'mdl-list'
	Then I click the Advanced filter options button
	Then I select the Status All filter option and wait for results
	Then I search Track for my Unique Ticket Number
	Then I click the search result link for my Unique Ticket Number

	#14 Verify the ticket is closed complete
	Then I verify my Ticket status is 'Closed Complete'
	
