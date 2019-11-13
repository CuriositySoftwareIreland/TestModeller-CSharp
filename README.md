# TestModeller.io C#

C# client library for the Test Modeller platform.

Perform and retrieve data allocations, synchronise run results, and execute jobs.

Include in your NuGet project using the following dependancy
https://www.nuget.org/packages/TestModeller.io/


```
Install-Package TestModeller.io -Version 1.0.0
```

## Data Allocation
Test data allocation works in three phrases. 

1. The test data required must be specified and exposed as a test data criterion within a data catalogue.

2. The automation framework must call the data catalogue API to execute and run the data allocations (this is what finds and makes the right data then assigns it to each test instance). 

3. Each test can retrieve the allocated data values which can then be used within the automation framework to perform the required actions across user-interfaces, APIs, mainframes, ETL environments, and much more. This is built to plug directly into an automation framework independent of the language or type of automation being executed.

### NUnit Frameworks
With Nunit you can tag each test with a ‘DataAllocation’ annotation.

```c#
[DataAllocation("poolName", "suiteName", (new[] { "groups"}))]
Public void testDefinition()
{
   …………
}
```

Here you can specify the data allocation to connect the test with. This corresponds to three parameters:
1.	poolName – Name of the allocation pool the tests reside in.

2.	suiteName – Name of the test suite the test resides in.

3.	groups – The tests to perform allocation on. These are the allocation tests associated with the current test being tagged. Wildcards can be used to match multiple test names. The groups tag also takes a list so multiple test types can be specified.

These three parameters must match the data values specified for each matching test case specified within the appropriate allocation pool within the portal.

Within the test case you can retrieve the results using the 'dataAllocationEngine.GetDataResult’ function. Here you can specify the pool, suite name, and test name to retrieve the results for. Again, this must match the specifications given in the associated allocation pool within the portal. The DataAllocationResult class contains the functions to retrieve results by the column names, and column indexes as specified in the initial test criteria.

```c#
DataAllocationResult allocResult = dataAllocationEngine.GetDataResult("pool", "suite", "test name");
```

Before the tests are executed in NUnit we have defined a SetUpFixture function which is executed before all the specified tests are executed. Within this function we collect all DataAllocate functions tagged against any tests about to be executed and then call the data allocation API to perform the associated executions. 

It is more efficient to perform these operations in bulk which is why they are collected into one list and then sent for allocation as opposed to directly performing the allocation inside each individual script. 

This implementation can be transposed to other testing frameworks (e.g. Nunit, Junit, etc) by replacing the appropriate keywords (SetUpFixture, and Test) with their corresponding values. The purpose of this C# library is to provide a set of out-the-box methods for enabling you to call the data allocation API within your framework seamlessly.

```c#
    [SetUpFixture]
    public class DataAllocSetup
    {
        [OneTimeSetUp]
        public void performAllocations()
        {
            ConnectionProfile cp = new ConnectionProfile();
            cp.APIKey = ModellerConfig.APIKey;
            cp.Url = ModellerConfig.APIUrl;

            DataAllocationEngine dataAllocationEngine = new DataAllocationEngine(cp);

            // Create a list of all the pools that need allocating
            List<AllocationType> allocationTypes = new List<AllocationType>();

            foreach (Test curTest in TestExecutionContext.CurrentContext.CurrentTest.Tests)
            {
                foreach (Test subTest in curTest.Tests)
                {
                    DataAllocation[] allocAttr = subTest.Method.GetCustomAttributes<DataAllocation>(true);
                    if (allocAttr != null && allocAttr.Length > 0)
                    {
                        foreach (String testType in allocAttr[0].groups)
                        {
                            AllocationType allocationType = new AllocationType(allocAttr[0].poolName, allocAttr[0].suiteName, testType);

                            allocationTypes.Add(allocationType);
                        }
                    }
                }
            }

            if (allocationTypes.Count > 0)
            {
                if (!dataAllocationEngine.ResolvePools(ModellerConfig.ServerName, allocationTypes))
                {
                    throw new Exception("Error - " + dataAllocationEngine.getErrorMessage());
                }
            }
        }
    }

```




