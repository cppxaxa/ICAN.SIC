# SETTINGS FOR RUNNING THE EXE - APP.CONFIG.XML

1. You can modify the app.config.xml file or the executablename.config.xml file as follows

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup>
	<runtime>  
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">  
			<probing privatePath="DLL;3rdParty"/>  
		</assemblyBinding>  
	</runtime>
</configuration>
```

2. The probing tag tells the directory to search for DLLs.




# GUIDELINES AND ASSUMPTIONS

1. In BrokerHub.cs, we find the PrimaryNamespace by sorting according to "ICAN.SIC" and "ICAN.SIC.Plugin"
2. We instantiate the plugin whose namespace word after last dot (.) is same as the class name
3. We plan to design the components with a Utility, Helper and the Plugin itself for adaptability to testing frameworks
4. Each component can have a "Host" project to instantiate and test it for developer
5. The "Host" can directly reference the component, for rest it's strict to reference using DLL



# Plugin for BrokerHub or IPlugin or AbstractPlugin

Every plugin needs to implement the IPlugin. For convenience, AbstractPlugin contains the nescessary code.  
AbstractPlugin also implements IPlugin. So, it's enough to implement the AbstractPlugin to make plugin.  
An abstract plugin can be casted to IPlugin anytime.  
AbstractPlugin features a protected "hub" to expose Subscribe<> and Publish<> methods inside class.  


# Some core Plugins

* SIMLHub (work in progress...)
* ChatRESTApi + Self hosted html page (to be constructed...)


# SIMLHub Plugins

SIMLHub can be extended using it's own plugins that implement "xxxx"


# SIMLHub Plugin - Directory Structure

Index.siml
IndexAdapter.dll

+ Fetch
	- Fetch.siml
	- FetchAdapter.dll

+ Filter
	- Filter.siml
	- FilterAdapter.dll

+ Select
	- Select.siml
	- SelectAdapter.dll

+ Operation
	- Operation.siml
	- OperationAdapter.dll


# BrokerHub Initialization

There needs to be a utility to flatten all the plugins directory to one with random names.  
This will help to run all the plugins.  
We need to keep track of our core dlls because we want to support reload and reinitialization.  
During reinitialization, we will delete all the files except core dlls and then
we will proceed with copy operation again.  

