using CuriositySoftware.DataAllocation.Entities;
using CuriositySoftware.JobEngine.Entities;
using CuriositySoftware.JobEngine.Services;
using CuriositySoftware.Utilities;
using CuriositySoftware.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CuriositySoftware.DataAllocation.Engine
{
    public class DataAllocationEngine
    {
        public String ErrorMessage { get; set; }

        public Boolean failOnError { get; set; }

        public List<AllocationType> allocationTypeList { get; set; }

        public ConnectionProfile ConnectionProfile { get; set; }

        public DataAllocationEngine()
        {
            this.allocationTypeList = new List<AllocationType>();

            failOnError = true;
        }

        public DataAllocationEngine(ConnectionProfile p)
        {
            this.ConnectionProfile = p;

            this.allocationTypeList = new List<AllocationType>();

            failOnError = true;
        }

        /**
         * Set failure on data resolution error
         * @param failOnError fail on error flag
         */
        public void setFailOnError(Boolean failOnError) {
            this.failOnError = failOnError;
        }

        /**
         * Get failure on data resolution error
         * @return failure on data resolution
         */
        public Boolean getFailOnError() {
            return failOnError;
        }

        /**
         * Get error message
         * @return error message
         */
        public String getErrorMessage() {
            return ErrorMessage;
        }

        public void AddAllocationType(String pool, String suite, String test)
        {
            allocationTypeList.Add(new AllocationType(pool, suite, test)); 
        }

        public void ClearAllocation()
        {
            allocationTypeList.Clear();
        }

        /**
         * Resolve specified tests within data pools on specified server
         * @param serverName server to use for performing resolution
         * @param allocationTypes allocations to perform
         * @return success or failure
         */
        public Boolean ResolvePools(String serverName, List<AllocationType> allocationTypes)
        {
            return ResolvePools(serverName, allocationTypes, 1000000000L);
        }

        public Boolean ResolvePools(String serverName)
        {
            return ResolvePools(serverName, allocationTypeList, 1000000000L);
        }

        /**
         * Resolve specified tests within data pools on specified server
         * @param serverName server to use for performing resolution
         * @param allocationTypes allocations to perform
         * @param maxTime maximum time in milliseconds to wait for resolution
         * @return success or failure
         */
        public Boolean ResolvePools(String serverName, List<AllocationType> allocationTypes, long maxTime)
        {
            // Chunk this into each pool ID
            Dictionary<String, List<AllocationType>> allocationPoolsToResolve = new Dictionary<String, List<AllocationType>>();
            foreach (AllocationType allocType in allocationTypes) {
                if (!allocationPoolsToResolve.ContainsKey(allocType.poolName))
                    allocationPoolsToResolve.Add(allocType.poolName, new List<AllocationType>());

                allocationPoolsToResolve[allocType.poolName].Add(allocType);
            }

            foreach (String poolName in allocationPoolsToResolve.Keys) {
                if (!performAllocation(serverName, poolName, allocationPoolsToResolve[poolName], maxTime))
                    if (failOnError)
                        return false;
            }

            return true;
        }

        /**
         * Retrieve data allocation result for test name within a test suite and data pool
         * @param pool pool to use for resolution
         * @param suite suite to use for resolution
         * @param testName test name of data to retrieve
         * @return allocated result
         */
        public DataAllocationResult GetDataResult(String pool, String suite, String testName)
        {
            RestClient client = new RestClient(this.ConnectionProfile.Url);
            client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

            RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/" + pool + "/suite/" + suite + "/allocated-test/" + testName + "/result/value", Method.GET);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;

            try
            {
                IRestResponse<DataAllocationResult> response = client.Execute<DataAllocationResult>(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    ErrorMessage = "Failed : HTTP error code - GetDataResult : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed : - GetDataResult : " + e.Message;

                Console.WriteLine(ErrorMessage);
            }

            return null;
        }

        private Boolean performAllocation(String serverName, String poolName, List<AllocationType> allocationTypes, long maxTimeMS)
        {
            JobSubmissionService jobSubmission = new JobSubmissionService(this.ConnectionProfile);

            // We'll need to package these up and call API to start job
            JobEntity curJobStatus = createAllocateJob(serverName, poolName, allocationTypes);
            if (curJobStatus == null)
            {
                ErrorMessage = jobSubmission.ErrorMessage;

                return false;
            }

            long? jobId = curJobStatus.id;

            // Wait for job to finish and complete
            long startTime = Environment.TickCount;
            while (true)
            {
                long ellapsed = Environment.TickCount - startTime;

                if (ellapsed > maxTimeMS) {
                    ErrorMessage = "Maximum time elapsed";

                    Console.WriteLine(ErrorMessage);

                    return false;
                }

                curJobStatus = jobSubmission.GetJob(jobId.Value);

                if (curJobStatus == null)
                    break;

                if (curJobStatus.jobState.Equals(JobState.Complete)){
                    Console.WriteLine("Executing job - State: " + curJobStatus.jobState + " - Message: " + curJobStatus.progressMessage);

                    return true;

                } else if (curJobStatus.jobState.Equals(JobState.Error)) {
                    ErrorMessage = "Error executing job " + curJobStatus.progressMessage;

                    Console.WriteLine(ErrorMessage);

                    return false;
                }

                Console.WriteLine("Executing job - State: " + curJobStatus.jobState + " - Message: " + curJobStatus.progressMessage);

                Thread.Sleep(2000);
            }

            Console.WriteLine("Executing job - State: " + curJobStatus.jobState + " - Message: " + curJobStatus.progressMessage);

            return false;
        }

        private JobEntity createAllocateJob(String serverName, String poolName, List<AllocationType> allocationTypes)
        {
            RestClient client = new RestClient(this.ConnectionProfile.Url);
            client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

            RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/" + poolName + "/resolve/server/" + serverName + "/execute", Method.POST);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(allocationTypes);

            try
            {
                IRestResponse<JobEntity> response = client.Execute<JobEntity>(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    Console.WriteLine("Failed : HTTP error code - createAllocateJob : " + response.Content);

                    return null;
                }

                return response.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed : - createAllocateJob : " + e.Message);
            }

            return null;
        }
    }
}
