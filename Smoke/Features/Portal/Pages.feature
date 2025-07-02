Feature: Smoke.Features.Portal.Pages
	In order to know that a particular page is available
	As a ServiceNow Developer
	I expect the page to load successfully



@dev @patch @uat @prod @video
Scenario Outline: PortalPage
	Given I go to this url '<Url>' and wait until I see the element with the selector '<Selector>'
	Then  I should observe no script errors on the page
Scenarios: 
| Name                    | Url                           | Selector                    | SelectorDescription |
| *HomePage*              | rttms                         | .portal-contentHeader       | page heading        |
| *TrackSearch*           | rttms/#/track/search          | .mdl-list                   | result list <ul>    |
| *OrderSearch*           | rttms/#/order/search          | .mdl-list                   | result list <ul>    |
| *HelpSearch*            | rttms/#/help/search           | .mdl-list                   | result list <ul>    |
| *Search*                | rttms/#/search/all            | #term                       | search text box     |
| *News*                  | rttms/#/news                  | .rtp_content-main           | content container   |
| *ProfileEdit*           | rttms/#/profile/edit          | .rtp_formField              | form field          |
| *ProfileSystemSettings* | rttms/#/profile/system        | .rtp_card__content__section | content section     |
| *ProfileTimeZone*       | rttms/#/profile/timezone      | .rtp_card__content__section | content section     |
| *ProfileServiceLevels*  | rttms/#/profile/servicelevels | .mdl-grid                   | content grid        |



