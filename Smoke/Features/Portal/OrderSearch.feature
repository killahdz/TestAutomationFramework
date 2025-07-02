Feature: Smoke.Features.Portal.OrderSearch
	As a portal user
	I want to be able to navigate to the Order page
	And search for a form
	Then open it see the form has no errors

@dev @patch @uat @prod @video
Scenario Outline: OrderSearch
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Given I go to the Portal Order page and wait for it to load
	Then I wait for 2 seconds
	Then I dismiss notifications
	Then I type '<FormTitle>' in the Search text box 	
	And  I wait for search results to appear		
	When I click the search result with title '<FormTitle>' and wait for the form to load
	Then I should observe no script errors on the page

Scenarios: 
| Name                              | FormTitle                           |
| *TravelForm*                      | Request Rio Tinto Projects Travel   |
#| *RequestLaptop*                   | Request Laptop                      |
| *RequestGeneralIT*                | Request General IT Assistance       |
#| *ContractorEntry*                 | Request Contractor Entry            |
| *ChangeContactDetails*            | Change Business Contact Details     |
| *RenewSoftwareLicence*            | Renew Software Licence or Agreement |
| *ReportRemoteAccessProblem*       | Report Remote Access Problem        |
| *RequestTranslationService*       | Request Translation Service         |
#| *RequestChangetoName*             | Request Change to Name              |
#| *RequestFurniture*                | Request Furniture                   |
| *RequestInternetAccess*           | Request Internet Access             |
#| *RequestDataCard*                 | Request Data Card                   |
#| *RequestMobilePhone*              | Request Mobile Phone                |
| *RequestSoftwareInstall*          | Request Software Install            |
| *CreateExternalAccessUserAccount* | Create External Access User Account |
| *ReportBusinessSolutionProblem*   | Report Business Solution Problem    |
#| *RequestParking*                  | Request Parking                     |
| *UninstallSoftware*               | Uninstall Software                  |
| *ReportCyberSecurityIncident*     | Report a Cyber Security Incident    |
| *CreateFileShare*                 | Create File Share                   |

