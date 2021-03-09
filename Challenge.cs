using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using NodaTime;
using NodaTime.Text;

namespace Challenge
{
    /// <summary>
    /// Boilerplate challenge class
    /// Check the TODO's in the file to check what needs to be changed
    /// Make sure not to change anything else so that we can test this against our input.
    /// </summary>
    public class Challenge
    {
        private static string csvFilePath;

        /// <summary>
        /// Comand line will be used to test the results
        /// During execution two prints will be done, one with the elapsed time and other with the resulting paid time
        /// The paid time value will be checked against our data set and expected results.
        /// The dates have instant iso format yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        /// <param name="args">the arguments will be a path for the CSV file containing the data set dump, a start date, end date</param>
        ///             and the agent id
        public static void Main(string[] args)
        {
            csvFilePath = args[0];
            Instant startInstant = InstantPattern.General.Parse(args[1]).Value;
            Instant endInstant = InstantPattern.General.Parse(args[2]).Value;
            long agentId = long.Parse(args[3]);
            Console.WriteLine("Time paid in seconds for agent "
                              + agentId
                              + " start = "
                              + startInstant
                              + " end = "
                              + endInstant
                              + " : "
                              + CalculatePaidTimeForAgent(startInstant, endInstant, agentId).TotalSeconds);
        }

        /// <summary>
        /// There are a few assumptions:
        /// - There are only two entities, the agent and the events which are in a ManyToOne relationship with the
        /// Employee entity.
        /// - For consistency it's assumed that no two events with the same priority for the same agent overlap.
        /// As this is a more general case and algorith to solve as such we:
        /// - Don't assume that the 0 priority events fully contain the others (the shift scenario)
        /// - Don't assume that the same priority events are all either paid or unpaid
        /// We do assume that all the events are fully contained in the filter start and end times.
        /// Given this we can make a query to fetch the data already sorted for us.
        /// We will order it by priority desc, start time asc.
        /// </summary>
        /// <param name="start">no timespan can start before this time</param>
        /// <param name="end">  no timespan can end after this time</param>
        /// <param name="agent">the agent (here is an id for simplification)</param>
        /// <returns>the amount of time that is paid given the filter criteria</returns>
        private static Duration CalculatePaidTimeForAgent(Instant start, Instant end, long agent) =>
            CalculatePaidTimeMeasured(SortEvents(FetchEventsForAgent(start, end, agent)));

        /// <summary>
        /// Mocking of database fetching
        /// Keep this method as is
        /// </summary>
        /// <param name="start">  no events will start prior to this instant</param>
        /// <param name="end">    no event will end past this instant</param>
        /// <param name="agentId">the agentId to which the events pertain to</param>
        /// <returns>the list of events filtered accordingly</returns>
        private static List<Event> FetchEventsForAgent(Instant start, Instant end, long agentId) =>
            AllEvents().Where(e => !(e.Start < start || e.End > end)
                                   && e.AgentId == agentId)
                       .ToList();

        /// <summary>
        /// Some example events that could be in the database.
        /// </summary>
        /// <returns>the boilerplate list of events for testing purposes</returns>
        private static IEnumerable<Event> AllEvents()
        {
            using(var streamReader = new StreamReader(csvFilePath))
            using (var reader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {
                return reader.GetRecords<Event>().ToList();
            }
        }


        private static Duration CalculatePaidTimeMeasured(List<Event> events)
        {
            Instant start = SystemClock.Instance.GetCurrentInstant();
            Duration paidTime = CalculatePaidTime(events);
            Instant end = SystemClock.Instance.GetCurrentInstant();
            Console.WriteLine("Time elapsed on algorithm: " + (end - start).TotalMilliseconds);
            return paidTime;
        }

        /// <summary>
        /// We'll assume that you queried the database with the sorting you needed but for testing purposes we ask that you
        /// implement your required sorting since you don't know the order of the input that this is going to be tested
        /// against
        /// </summary>
        /// <param name="events">unordered events</param>
        /// <returns>ordered events</returns>
        private static List<Event> SortEvents(List<Event> events)
        {
            //TODO implement your order of the events, leave this as is if your algorithm does not care about event order
            return events;
        }

        /// <summary>
        /// Calculates the amount of paid time taking into account the events priority.
        /// Assume you have a list of Event coming in from a database with the query of your choosing
        /// Meaning that you have it filtered and ordered as you want, implement the order in Challenge#orderEvents
        /// TODO don't forget to comment and document the code to portray the assumptions that were made
        /// </summary>
        /// <param name="events">events to considered already filtered and ordered</param>
        /// <returns>a duration representing the amount of time that is to be paid for the events</returns>
        private static Duration CalculatePaidTime(List<Event> events)
        {
            return Duration.Zero;

            //TODO implement algorithm here
        }
    }
}