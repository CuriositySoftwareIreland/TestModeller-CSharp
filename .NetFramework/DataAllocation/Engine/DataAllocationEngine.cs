using CuriositySoftware.DataAllocation.Entities;
using CuriositySoftware.JobEngine.Entities;
using CuriositySoftware.JobEngine.Services;
using CuriositySoftware.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestModeller_CSharp.DataAllocation.Entities;
using TestModeller_CSharp.DataAllocation.Engine;
using TestModeller_CSharp.DataAllocation.Services;

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
            return GetDataResult(pool, suite, testName, ResultMergeMethod.NoMerge);
        }

        /**
         * Retrieve data allocation result for test name within a test suite and data pool
         * @param pool pool to use for resolution
         * @param suite suite to use for resolution
         * @param testName test name of data to retrieve
         * @param mergeMethod method to merge test data
         * @return allocated result
         */
        public DataAllocationResult GetDataResult(String pool, String suite, String testName, ResultMergeMethod mergeMethod)
        {
            AllocationLookupDto lookupDto = new AllocationLookupDto();
            lookupDto.pool = pool;
            lookupDto.suite = suite;
            lookupDto.testName = testName;

            RestClient client = new RestClient(this.ConnectionProfile.Url);
            client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

            RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/suite/allocated-test/result/value", Method.POST);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(lookupDto);
            request.AddQueryParameter("mergeMethod", mergeMethod.ToString());

            try
            {
                IRestResponse<DataAllocationResult> response = client.Execute<DataAllocationResult>(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    ErrorMessage = "Failed : HTTP error code - GetDataResult : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }

                Console.WriteLine("Found data for " + pool + " " + suite + " " + testName);
                Console.WriteLine(response.Data.ToString());

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed : - GetDataResult : " + e.Message;

                Console.WriteLine(ErrorMessage);
            }

            return null;
        }


        public DataAllocationResult GetDataResultForCriteriaInPool(String serverName, String poolName, String catalogueName, String criteriaName, Int32 howMany, List<DataAllocationCriteria> criteriaParameters)
        {
            return GetDataResultForCriteriaInPool(serverName, poolName, catalogueName, criteriaName, howMany, criteriaParameters, 10000000L);
        }

        public DataAllocationResult GetDataResultForCriteriaInPool(String serverName, String poolName, String catalogueName, String criteriaName, Int32 howMany, List<DataAllocationCriteria> criteriaParameters, long maxTime)
        {
            // 1) Find criteria of name in catalogue
            DataCriteriaService dataCriteriaService = new DataCriteriaService(ConnectionProfile);
            DataCatalogueTestCriteria criteria = dataCriteriaService.GetTestCriteria(catalogueName, criteriaName);
            if (criteria == null)
            {
                ErrorMessage = dataCriteriaService.getErrorMessage();

                Console.WriteLine("Error GetTestCriteria() " + ErrorMessage);

                return null;
            }

            // 2) Find the existing allocation pool
            AllocationPoolService allocationPoolService = new AllocationPoolService(ConnectionProfile);
            AllocationPool allocationPool = allocationPoolService.GetAllocationPool(poolName);
            if (allocationPool == null)
            {
                ErrorMessage = allocationPoolService.getErrorMessage();

                Console.WriteLine("Error GetAllocationPool() - " + ErrorMessage);

                return null;
            }

            // 3) Create a new test inside pool
            AllocatedTest allocatedTest = new AllocatedTest();
            allocatedTest.howMany = (howMany);
            allocatedTest.name = ("Test " + Guid.NewGuid().ToString());
            allocatedTest.poolId = (allocationPool.id);
            allocatedTest.suiteName = ("DataAllocationFramework");
            allocatedTest.testCriteriaIdCatalogueId = (allocationPool.catalogueId);
            allocatedTest.testCriteriaId = (criteria.id);
            allocatedTest.uniqueFind = (false);

            Dictionary<String, DataCatalogueModellerParameter> criteriaParamHash = criteria.getModellerParameterHash();

            List < AllocatedTestParameter > allocParams = new List<AllocatedTestParameter>();
            foreach (DataAllocationCriteria allocationCriteria in criteriaParameters)
            {
                AllocatedTestParameter param = new AllocatedTestParameter();

                if (criteriaParamHash.ContainsKey(allocationCriteria.parameterName))
                {
                    param.criteriaParameterId = (criteriaParamHash[(allocationCriteria.parameterName)].id);
                    param.value = (allocationCriteria.parameterValue);
                    param.criteriaParameterName = (allocationCriteria.parameterName);

                    allocParams.Add(param);
                }
            }
            allocatedTest.parameters = (allocParams);

            AllocationPoolTestService allocationPoolTestService = new AllocationPoolTestService(ConnectionProfile);
            allocatedTest = allocationPoolTestService.CreateAllocatedTest(allocatedTest, allocationPool.id);
            if (allocatedTest == null)
            {
                ErrorMessage = dataCriteriaService.getErrorMessage();

                Console.WriteLine("Error CreateAllocatedTest() " + ErrorMessage);

                return null;
            }


            // 4) Run Allocation and retrieve result
            List<AllocationType> allocationTypes = new List<AllocationType>();
            allocationTypes.Add(new AllocationType(allocationPool.name, allocatedTest.suiteName, allocatedTest.name));
            performAllocation(serverName, allocationPool.name, allocationTypes, maxTime);

            DataAllocationResult res = GetDataResult(allocationPool.name, allocatedTest.suiteName, allocatedTest.name);

            // 5) Delete allocatedTest
            Boolean deleted = allocationPoolTestService.DeleteAllocationPoolTest(allocatedTest.id);
            if (!deleted)
            {
                ErrorMessage = allocationPoolTestService.getErrorMessage();

                Console.WriteLine("Error DeleteAllocationPoolTest() " + ErrorMessage);

                return res;

            }

            return res;
        }

        public DataAllocationResult getDataResultForCriteria(String serverName, String catalogueName, String criteriaName, Int32 howMany, List<DataAllocationCriteria> criteriaParameters)
        {
            return getDataResultForCriteria(serverName, catalogueName, criteriaName, howMany, criteriaParameters, 10000000L);
        }

        public DataAllocationResult getDataResultForCriteria(String serverName, String catalogueName, String criteriaName, Int32 howMany, List<DataAllocationCriteria> criteriaParameters, long maxTime)
        {
            // 1) Find criteria of name in catalogue
            DataCriteriaService dataCriteriaService = new DataCriteriaService(ConnectionProfile);
            DataCatalogueTestCriteria criteria = dataCriteriaService.GetTestCriteria(catalogueName, criteriaName);
            if (criteria == null)
            {
                ErrorMessage = dataCriteriaService.getErrorMessage();

                Console.WriteLine("Error GetTestCriteria() " + ErrorMessage);

                return null;
            }

            // 2) Create a new allocation pool
            AllocationPool allocationPool = new AllocationPool();
            allocationPool.catalogueId = (criteria.catalogueId);
            allocationPool.name = ("Temporary pool " + Guid.NewGuid().ToString());

            AllocationPoolService allocationPoolService = new AllocationPoolService(ConnectionProfile);
            allocationPool = allocationPoolService.CreateAllocationPool(allocationPool);
            if (allocationPool == null)
            {
                ErrorMessage = dataCriteriaService.getErrorMessage();

                Console.WriteLine("Error CreateAllocationPool() " + ErrorMessage);

                return null;
            }

            // 3) Create a new test inside pool
            AllocatedTest allocatedTest = new AllocatedTest();
            allocatedTest.howMany = (howMany);
            allocatedTest.name = ("Test " + Guid.NewGuid().ToString());
            allocatedTest.poolId = (allocationPool.id);
            allocatedTest.suiteName = ("DataAllocationFramework");
            allocatedTest.testCriteriaIdCatalogueId = (allocationPool.catalogueId);
            allocatedTest.testCriteriaId = (criteria.id);
            allocatedTest.uniqueFind = (false);

            Dictionary<String, DataCatalogueModellerParameter> criteriaParamHash = criteria.getModellerParameterHash();

            List < AllocatedTestParameter > allocParams = new List<AllocatedTestParameter>();
            foreach (DataAllocationCriteria allocationCriteria in criteriaParameters)
            {
                AllocatedTestParameter param = new AllocatedTestParameter();

                if (criteriaParamHash.ContainsKey(allocationCriteria.parameterName))
                {
                    param.criteriaParameterId = (criteriaParamHash[(allocationCriteria.parameterName)].id);
                    param.value = (allocationCriteria.parameterValue);
                    param.criteriaParameterName = (allocationCriteria.parameterName);

                    allocParams.Add(param);
                }
            }
            allocatedTest.parameters = (allocParams);

            AllocationPoolTestService allocationPoolTestService = new AllocationPoolTestService(ConnectionProfile);
            allocatedTest = allocationPoolTestService.CreateAllocatedTest(allocatedTest, allocationPool.id);
            if (allocatedTest == null)
            {
                ErrorMessage = dataCriteriaService.getErrorMessage();

                Console.WriteLine("Error CreateAllocatedTest() " + ErrorMessage);

                return null;
            }


            // 4) Run Allocation and retrieve result
            List<AllocationType> allocationTypes = new List<AllocationType>();
            allocationTypes.Add(new AllocationType(allocationPool.name, allocatedTest.suiteName, allocatedTest.name));
            performAllocation(serverName, allocationPool.name, allocationTypes, maxTime);

            DataAllocationResult res = GetDataResult(allocationPool.name, allocatedTest.suiteName, allocatedTest.name);

            // 5) Delete pool
            Boolean deleted = allocationPoolService.DeleteAllocationPool(allocationPool.id);
            if (!deleted)
            {
                ErrorMessage = allocationPoolService.getErrorMessage();

                Console.WriteLine("Error DeleteAllocationPool() " + ErrorMessage);

                return res;

            }

            return res;

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
