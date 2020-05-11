# CalculateWeekdaysApi

This is an Asp.Net Core 3.1 WebAPI for Calculating the Weekdays for given valid date range. The API is meant to calculate the working days while excluding the start and end date, and all the weekends and public holidays.

### Scope
The API is meant to calculate the working days or weekdays between a given date range in the format of (dd/mm/yyyy). The API does not maitain user state and hence is stateless. The API calls another third party API to get the valid *national holiday list for Australia*. This 3rd Party API is: https://getfestivo.com/. Currently the **CalculateWeekdaysApi** is using a free tier version of the GetFestivo API and hence a database repository is maintained to store the holiday list for a queried year  Get. So for instance, if the user queries for a certain date range that does not exist in database, the GetFestivo API will be called once. *Once the holiday list is received, the list and the year is upserted into the database.* **All subsequent queries for that year will be done on database and not on API.** 
