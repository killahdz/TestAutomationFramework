Feature: Security.Features.Portal.RequestFor
	In order to request a form for a different user
	I need to be able to select a user to request for in the portal view


@dev @video
Scenario: RequestFor.OtherUser.GeneralITRequest
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Given I go to this url 'rttms/#/order/search' and wait until I see the element with this class 'mdl-list'
	Then I dismiss notifications
	When I request for 'Daniel Kereama'
	Then I should see 'Kereama, Daniel (IST-PIPEFISHPL)' as the active requestor
