# CalculateWeekdaysApi

This is an Asp.Net Core 3.1 REST WebAPI for Calculating the Weekdays for given valid date range. The API is meant to calculate the working days while excluding the start and end date, and all the weekends and public holidays that fall on weekdays.


### Scope of the API
The API is meant to calculate the working days or weekdays between a given date range in the format of (dd/mm/yyyy). The API does not maitain user state and hence is stateless. The API uses data structure based implementation to calculate the holidays. As for the current version the database is setup to include following holidays for ***any year***:
<ul>
<li>Australia Day</li>
<li>Anzac Day</li>
<li>Good Friday</li>
<li>Queen's Birthday</li>
<li>Christmas Day</li>
<li>Boxing Day</li>
</ul>


### Tech Stack
For API development: .Net Core 3.1, Entity Framework 6.0 (Lazy Loading) <br/>
For Unit Testing: NUnit, Moq <br/>
For Logging: NLog <br/>
For API XML Documentation: Swagger/Swashbuckle Asp.Net Core<br/>


### API Endpoints

* Post()<br/>
 Input start date and end date in dd/mm/yyyy format.
 ##### MethodVerb: `Post` <br/>
 ##### Sample Request: 
 ```
 {
	"StartDate" : "13/8/1999",
	"EndDate": "31/12/1999"
}
```


### Assumptions and Considerations
1. The start and end dates are mandatory fields for the API and appropriate validation logic has been put in place for it.
2. The start and end date have to be entered in **dd/mm/yyyy** format as per applicable to .NET CultureInfo “English (Australia)”.
3. If start date and end date are same, response returned is 0.

