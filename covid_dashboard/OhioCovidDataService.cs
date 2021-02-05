using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace covid_dashboard
{
	/// <summary>
	/// Static Class used to interact and gather data from a backend mysql server.
	/// </summary>
	static class OhioCovidDataService
	{

		private static MySqlConnection conn = null;
		/// <summary>
		/// Opens the MySqlConnection object
		/// </summary>
		/// <returns>An open MySqlConnection object connected to the database determined in the App.config file</returns>
		private static MySqlConnection openConnection()
		{
			//connect and open the connection if it is null or closed
			if (conn == null || conn.State == System.Data.ConnectionState.Closed)
			{
				conn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ocdConn"].ConnectionString);
				conn.Open();
			}
			return conn;
		}
		/// <summary>
		/// For use upon startup of app. Pulls all the data from the backend and puts it in a Dictionary with the name of the county as the key and the corresponding CountyData object as the value
		/// </summary>
		/// <param name="sender">BackgroundWorker object to show progress of pulling data in a progress bar in WPF</param>
		/// <returns>Dictionary holding all recorded data</returns>
		public static Dictionary<string, CountyData> getDataStartUp(BackgroundWorker sender)
		{
			Dictionary<string, CountyData> allData = new Dictionary<string, CountyData>(88);
			
			//get the number of rows to read in order to show progress bar in WPF
			openConnection();
			MySqlCommand countCommand = new MySqlCommand("select count(*) from data", conn);
			MySqlDataReader countDataReader = countCommand.ExecuteReader();
			int totalRows = 0;
			if (countDataReader.Read())
            {
				totalRows = Convert.ToInt32(countDataReader["count(*)"]);
			}
			countDataReader.Close();
			//perform query to get all data
			int count = 0;
			int percentage = 0;
			string commandString = "select * from data";
			MySqlCommand command = new MySqlCommand(commandString, conn);
			using (command)
			{
				MySqlDataReader dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					//update progress bar
					double actualPercentage = (double)count / totalRows;
					if ((int)actualPercentage > percentage)
                    {
						percentage = (int)actualPercentage;
						sender.ReportProgress(percentage);
                    }
					count++;

					//format and store data gathered from database
					int dbCount = Convert.ToInt32(dataReader["count"]);
					int dbHosp = Convert.ToInt32(dataReader["hosp_count"]);
					int dbDeaths = Convert.ToInt32(dataReader["death_count"]);
					string onsetDate = Convert.ToString(dataReader["onset_date"]);
					string dateCollected = Convert.ToString(dataReader["date_collected"]);
					string timeCollected = Convert.ToString(dataReader["time_collected"]);
					string sex = Convert.ToString(dataReader["sex"]);
                    string ageRange = Convert.ToString(dataReader["age_range"]);
					string county = Convert.ToString(dataReader["county"]);
					int cutoff = dateCollected.LastIndexOf("2020") + 4;
					dateCollected = dateCollected.Substring(0, cutoff).Trim();
					dateCollected += " " + timeCollected;
					DateTime collectedOn = DateTime.ParseExact(dateCollected, "M/d/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
					DataLine dL = new DataLine(sex, ageRange, onsetDate, dbCount, dbHosp, dbDeaths);
					if (allData.ContainsKey(county))
                    {
						allData[county].add(collectedOn, dL);
                    }
                    else
                    {
						allData.Add(county, new CountyData(county));
						allData[county].add(collectedOn, dL);
                    }
				}
				dataReader.Close();
			}
			conn.Close();
			return allData;
		}
		/// <summary>
		/// Returns data for all counties on a specific date. 
		/// </summary>
		/// <param name="date">Date to pull data for, date is assumed to be after 1400 on the day of the date given or before 1400 on the next day</param>
		/// <param name="startup">True if method is called upon startup of the app, will not return null in this case</param>
		/// <returns>Dictionary holding all recorded data on a specific date. Null if no data is present for that date.</returns>
		public static Dictionary<string, CountyData> getData(DateTime date, bool startup)
		{
			Dictionary<string, CountyData> allData = new Dictionary<string, CountyData>(88);
			//format date portion of mysql query based on the date parameter
			string datePortion = "date_collected='" + date.ToString("yyyy'-'MM'-'dd") + "' and time_collected>='14:00:00' or date_collected='"
				  + date.AddDays(1).ToString("yyyy'-'MM'-'dd") + "' and time_collected<'14:00:00'";
			openConnection();
			//Check to see if there is data for the date provided
			while (true)
            {
				MySqlCommand countCommand = new MySqlCommand("select count(*) from data where " + datePortion , conn);
				MySqlDataReader countDataReader = countCommand.ExecuteReader();
				if (countDataReader.Read())
				{
					//if count is zero, there is no data for date provided
					if (!(Convert.ToInt32(countDataReader["count(*)"]) > 0))
					{
						//if method is used at startup, go back one day until data is obtained
						if (startup)
						{
							date = date.AddDays(-1);
							datePortion = "date_collected='" + date.ToString("yyyy'-'MM'-'dd") + "' and time_collected>='14:00:00' or date_collected='"
								+ date.AddDays(1).ToString("yyyy'-'MM'-'dd") + "' and time_collected<'14:00:00'";
							countDataReader.Close();
						}
						//if method is not used at startup, return null
						else
						{
							return null;
						}
					}
					else
                    {
						//exit while loop after closing data reader
						countDataReader.Close();
						break;
                    }
				}
				
            }
			
			//query  and store data 
			string commandString = "select * from data where " + datePortion;
			MySqlCommand command = new MySqlCommand(commandString, conn);
			using (command)
			{
				MySqlDataReader dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					int dbCount = Convert.ToInt32(dataReader["count"]);
					int dbHosp = Convert.ToInt32(dataReader["hosp_count"]);
					int dbDeaths = Convert.ToInt32(dataReader["death_count"]);
					string onsetDate = Convert.ToString(dataReader["onset_date"]);
					string dateCollected = Convert.ToString(dataReader["date_collected"]);
					string timeCollected = Convert.ToString(dataReader["time_collected"]);
					string sex = Convert.ToString(dataReader["sex"]);
					string ageRange = Convert.ToString(dataReader["age_range"]);
					string county = Convert.ToString(dataReader["county"]);
					int cutoff = dateCollected.LastIndexOf("2021") + 4;
					dateCollected = dateCollected.Substring(0, cutoff).Trim();
					dateCollected += " " + timeCollected;
					DateTime collectedOn = DateTime.ParseExact(dateCollected, "M/d/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
					DataLine dL = new DataLine(sex, ageRange, onsetDate, dbCount, dbHosp, dbDeaths);
					if (allData.ContainsKey(county))
					{
						allData[county].add(collectedOn, dL);
					}
					else
					{
						allData.Add(county, new CountyData(county));
						allData[county].add(collectedOn, dL);
					}
				}
				dataReader.Close();
			}
			conn.Close();
			return allData;
		}



		




	}
}
