using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace covid_dashboard
{
	static class OhioCovidDataService
	{

		private static string connString = "";

		private static MySqlConnection conn = null;

		private static MySqlConnection openConnection()
		{
			if (conn == null || conn.State == System.Data.ConnectionState.Closed)
			{
				conn = new MySqlConnection(connString);
				conn.Open();
			}
			return conn;
		}

		//approx. 46-51 seconds as of 30Sep2020
		public static Dictionary<string, CountyData> getDataStartUp()
		{
			Dictionary<string, CountyData> allData = new Dictionary<string, CountyData>(88);
			//string commandString = "select * from data where date_collected > '2020-09-09'";
			string commandString = "select * from data";
			openConnection();
			//get the number of rows to read
			MySqlCommand countCommand = new MySqlCommand("select count(*) from data", conn);
			MySqlDataReader countDataReader = countCommand.ExecuteReader();
			int totalRows = 0;
			if (countDataReader.Read())
            {
				totalRows = Convert.ToInt32(countDataReader["count(*)"]);
			}
			countDataReader.Close();
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
		//approx. 6-8 seconds as of 30Sep2020
		public static Dictionary<string, CountyData> getDataStartUp(DateTime date)
		{
			Dictionary<string, CountyData> allData = new Dictionary<string, CountyData>(88);
			string datePortion = "date_collected='" + date.ToString("yyyy'-'MM'-'dd") + "' and time_collected>='14:00:00' or date_collected='"
				  + date.AddDays(1).ToString("yyyy'-'MM'-'dd") + "' and time_collected<'14:00:00'";
			openConnection();
			//get the number of rows to read
			MySqlCommand countCommand = new MySqlCommand("select count(*) from data where " + datePortion , conn);
			MySqlDataReader countDataReader = countCommand.ExecuteReader();
			if (countDataReader.Read())
			{
				if (!(Convert.ToInt32(countDataReader["count(*)"]) > 0))
                {
					date = date.AddDays(-1);
					datePortion = "date_collected='" + date.ToString("yyyy'-'MM'-'dd") + "' and time_collected>='14:00:00' or date_collected='"
						+ date.AddDays(1).ToString("yyyy'-'MM'-'dd") + "' and time_collected<'14:00:00'";
				}
			}
			countDataReader.Close();
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

		public static Dictionary<DateTime, int[]> getOhioData(DateTime selectedDate)
		{
			Dictionary<DateTime, int[]> data = new Dictionary<DateTime, int[]>();
			DateTime nextDay = selectedDate.AddDays(1);
			string datePortion = "date_collected>='" + selectedDate.ToString("yyyy'-'MM'-'dd") + "' and onset_date='" + selectedDate.ToString("yyyy'-'MM'-'dd") + "';";
			string commandString = "select county, date_collected, count, death_count, hosp_count from data where " + datePortion;
			openConnection();

			//see if there is data for selected dat

			MySqlCommand command = new MySqlCommand(commandString, conn);
			using (command)
			{
				MySqlDataReader dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					int dbCount = Convert.ToInt32(dataReader["count"]);
					int dbHosp = Convert.ToInt32(dataReader["hosp_count"]);
					int dbDeaths = Convert.ToInt32(dataReader["death_count"]);
					string dateCollected = Convert.ToString(dataReader["date_collected"]);
					string county = Convert.ToString(dataReader["county"]);
					int cutoff = dateCollected.LastIndexOf("2020") + 4;
					dateCollected = dateCollected.Substring(0, cutoff).Trim();
					DateTime collectedOn = DateTime.ParseExact(dateCollected, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
					if (data.ContainsKey(collectedOn))
					{
						data[collectedOn][0] += dbCount;
						data[collectedOn][1] += dbHosp;
						data[collectedOn][2] += dbDeaths;
					}
					else
					{
						data.Add(collectedOn, new int[] { dbCount, dbHosp, dbDeaths });
					}

				}
				dataReader.Close();
			}
			conn.Close();
			return data;

			

		}

		public static Dictionary<DateTime, int[]> getCountyData(DateTime selectedDate, string county)
		{
			Dictionary<DateTime, int[]> data = new Dictionary<DateTime, int[]>();
			DateTime nextDay = selectedDate.AddDays(1);
			string otherPortion = "county='" + county + "' and date_collected>='" + selectedDate.ToString("yyyy'-'MM'-'dd") + "' and onset_date='" + selectedDate.ToString("yyyy'-'MM'-'dd") + "';";
			string commandString = "select county, date_collected, count, death_count, hosp_count from data where " + otherPortion;
			openConnection();
			MySqlCommand command = new MySqlCommand(commandString, conn);
			using (command)
			{
				MySqlDataReader dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					int dbCount = Convert.ToInt32(dataReader["count"]);
					int dbHosp = Convert.ToInt32(dataReader["hosp_count"]);
					int dbDeaths = Convert.ToInt32(dataReader["death_count"]);
					string dateCollected = Convert.ToString(dataReader["date_collected"]);
					string countyName = Convert.ToString(dataReader["county"]);
					int cutoff = dateCollected.LastIndexOf("2020") + 4;
					dateCollected = dateCollected.Substring(0, cutoff).Trim();
					DateTime collectedOn = DateTime.ParseExact(dateCollected, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
					if (data.ContainsKey(collectedOn))
					{
						data[collectedOn][0] += dbCount;
						data[collectedOn][1] += dbHosp;
						data[collectedOn][2] += dbDeaths;
					}
					else
					{
						data.Add(collectedOn, new int[] { dbCount, dbHosp, dbDeaths });
					}

				}
			}
			conn.Close();
			return data;



		}

		




	}
}
