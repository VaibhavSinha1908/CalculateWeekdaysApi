# CalculateWeekdaysApi

This is an Asp.Net Core 3.1 REST WebAPI for Calculating the Weekdays for given valid date range. The API is meant to calculate the working days while excluding the start and end date, and all the weekends and public holidays that fall on weekdays.


### Scope of the API
The API is meant to calculate the working days or weekdays between a given date range in the format of (dd/mm/yyyy). The API does not maitain user state and hence is stateless. The API calls another third party API to get the valid *national holiday list for Australia*. This 3rd Party API used here is: https://getfestivo.com/. 

  Currently the **CalculateWeekdaysApi** is using a free tier version of the GetFestivo API and hence a database repository is maintained to store the holiday list for a queried year. So for instance, if the user queries for a certain date range that does not exist in database, the GetFestivo API will be called once for each year. *Once the holiday list is received, the holiday list and the year is upserted into the database.* **All subsequent queries for that year will be done on database and not on API.** This saves the API calls and results in *faster retrival of data and hence faster performance.*


### Tech Stack
For API development: .Net Core 3.1 <br/>
For Unit Testing: NUnit, Moq <br/>
For Logging: NLog <br/>
For API XML Documentation: Swagger/Swashbuckle Asp.Net Core<br/>


### Project URL:
The API has been deployed to a t2.micro EC2 server. The project URL is: <br/>
**http://ec2-13-54-131-88.ap-southeast-2.compute.amazonaws.com/swagger/index.html**


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
4. The total calls to 3rd Party API under free tier is limited to 100 per quarter. Since we are upserting the queried holiday information in the database, we can query 100 distinct years.
5. The CalculateWeekdaysApi API does not account for future dates i.e. 2021 and so on, as the GetFestivo API does not support future dates/year for the free api key.



### Scope for Improvements
1. The SQL server storing the holiday data is hosted within AWS to give a better performance when coupled with EC2 machine. For even better performance, ElastiCache (Redis) can be used.
2. Once the API key is formally purchased, the API key can be stored/retreived from the System Manager->Parameter Store of AWS for security.
