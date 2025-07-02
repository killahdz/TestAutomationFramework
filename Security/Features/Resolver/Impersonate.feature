Feature: Security.Features.Resolver.Impersonate
	In order to access the rttms system
	I need to be able to log in



@dev @prod @video
Scenario: Impersonate
	Given I am on the Login Form
	When I login with the username 'rttms.automated.testing' and password 'autopass'
	Then I Should be Logged in
	Then I impersonate as 'daniel kereama'
	Then I unimpersonate
	Then I Logout
