Feature: Smoke.Features.Resolver.Lists
	As a resolver, I want to be able to go to an item list 
	and open requests to verify there are no script errors

@dev @patch @uat @prod @video
Scenario Outline: ResolverList
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Then I go to the '<ListName>' List at '<Url>'
	Then I open a random list item and wait for the element with class '<Selector>'	
	Then I should observe no script errors on the page
	Then I go to the '<ListName>' List at '<Url>'
	Then I open a random list item and wait for the element with class '<Selector>'	
	Then I should observe no script errors on the page
	Then I go to the '<ListName>' List at '<Url>'
	Then I open a random list item and wait for the element with class '<Selector>'	
	Then I should observe no script errors on the page


	Scenarios: 
| Name                                      | ListName                           | Url                                                           | Selector  |
| *sc_req_item_list*                        | Requested Items List               | sc_req_item_list.do?sysparm_orderdesc=number                  | form_body |
| *sc_task_list*                            | Task List                          | sc_task_list.do?sysparm_orderdesc=number                      | form_body |
| *u_escalation_task_list*                  | Escalation Task List               | u_escalation_task_list.do?sysparm_orderdesc=number            | form_body |
| *incident_list*                           | Ticket List                        | incident_list.do?sysparm_orderdesc=number                     | form_body |
| *u_incident_task_list*                    | Ticket Task List                   | u_incident_task_list.do?sysparm_orderdesc=number              | form_body |
| *task_sla_list*                           | Task SLA List                      | task_sla_list.do?sysparm_orderdesc=number                     | form_body |
| *change_request_list*                     | Change Request List                | change_request_list.do?sysparm_orderdesc=number               | form_body |
| *new_call_list*                           | Calls List                         | new_call_list.do?sysparm_orderdesc=number                     | form_body |
| *hr_list*                                 | Human Resource Request List        | hr_list.do?sysparm_orderdesc=number                           | form_body |
| *u_general_request_list*                  | General Request List               | u_general_request_list.do?sysparm_orderdesc=number            | form_body |
| *u_general_request_finance_list*          | General Request Finance List       | u_general_request_finance_list.do?sysparm_orderdesc=number    | form_body |
| *dmn_demand_list*                         | Demand List                        | dmn_demand_list.do?sysparm_orderdesc=number                   | form_body |
| *pm_project_list*                         | Project List                       | pm_project_list.do?sysparm_orderdesc=number                   | form_body |
| *problem_list*                            | Problem List                       | problem_list.do?sysparm_orderdesc=number                      | form_body |
| *u_sts_request_list*                      | STS List                           | u_sts_request_list.do?sysparm_orderdesc=number                | form_body |
| *grc_control_list*                        | Control List                       | grc_control_list.do                                           | form_body |
| *grc_control_test_definition_list*        | Control Test Definition List       | grc_control_test_definition_list.do?sysparm_orderdesc=updated | form_body |
| *grc_control_test_list*                   | Control Test List                  | grc_control_test_list.do?sysparm_orderdesc=number             | form_body |
| *grc_remediation_list*                    | Remediation List                   | grc_remediation_list.do?sysparm_orderdesc=number              | form_body |
| *grc_audit_list*                          | Assurance Reviews List             | grc_audit_list.do?sysparm_orderdesc=number                    | form_body |
| *u_sp_site_profile_list*                  | Site Profile List                  | u_sp_site_profile_list.do?sysparm_orderdesc=number            | form_body |
| *u_sp_security_plan_list*                 | Security Plan List                 | u_sp_security_plan_list.do?sysparm_orderdesc=number           | form_body |
| *u_sp_security_risk_management_plan_list* | Security Risk Management Plan List | u_sp_security_risk_management_plan_list.do                    | form_body |
| *u_sp_plan_control_list*                  | Security Control List              | u_sp_plan_control_list.do                                     | form_body |
| *u_sp_self_assessment_list*               | Self Assessment List               | u_sp_self_assessment_list.do?sysparm_orderdesc=number         | form_body |
