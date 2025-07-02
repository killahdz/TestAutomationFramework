Feature: Forms.Features.Portal.CreateNewPosition
		As a portal user
		I want to know I can successfully 
		submit the Create New Position form

#@dev @patch @uat @video
Scenario: CreateNewPosition
	Given I am on the 'Create New Position' Form at 'rttms/#/order/item/Position_Create_NEW'
	Then I select the 'Direct selection' radio option from the 'search_type' field
	Then I select the 'AMT_Asset Management (AMT)' option in the 'ds_functional_area' list
	Then I select the 'AO_Asset Optimisation (AMT-AO)' option in the 'ds_job_family' list
	Then I select the 'Adviser I - Asset Optimisation (AMTAO10IC)' option in the 'ds_job' list
	Then I enter 'Big Boss Man' in the 'position_title' text field
	Then I select the 'Yes' radio option from the 'q_reports_to_position_team' field
	Then I select the 'No' radio option from the 'q_direct_reports' field
	Then I select the 'I will split the costs between multiple cost allocations' radio option from the 'q_cost_centre_distribution' field
	Then I select the 'I will charge the costs to a single cost centre' radio option from the 'q_cost_centre_distribution' field
	Then I enter '0010000000' in the 'cost_centre' lookup text field
	Then I select the 'Moving an employee into the position' option in the 'position_type' list
	Then I check the 'employee_mor_confirmed' check box field
	Then I select the 'No Change' option in the 'employee_emp_type_change' list
	Then I select the 'No change' option in the 'employee_residency_commute_status' list
	Then I select the 'Relocation not required' radio option from the 'employee_relocation_required' field
	When I click the form submit button and wait for the page to finish loading
	Then I should see the title 'Thank you, your request has been submitted'
	And I should observe no script errors on the page

