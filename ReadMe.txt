Linux Installation & Execution
	Check Ubuntu version
	lsb_release -a

	Install .Net SDK on Ubuntu
	curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
	sudo apt-add-repository https://packages.microsoft.com/ubuntu/[REPLACE BY RELEASE NO]/prod
	sudo apt-get update
	sudo apt-get install dotnet-sdk-3.1

	Confirm that .Net Core SDK properly installed
	dotnet --list-sdks

	Build projets, go in qohash solution folder
	donet build qohash.sln

	Run CLI, go in qohash.cli folder
	donet run

	Run Api, go in qohash.api folder
	donet run
	
	Run Web, go in qohash.web folder
	update the "BaseApiUrl": "http://localhost:59103" to reflect url where api is started
	donet run
	
	
Assomptions
	1. Event if an error occurred while reading a file or folder (i.e.: access denied or file already in used). 
	   I decided to continue process the rest of the scan. I also return all errors that have occured during a scan.

	2. All files and folders must be sorted all together by size
	
	3. Display in table form (name, size, lastWriteDate)
	
	4. Display total files number and total file size at the end
	
	5. Display error if any occured


Architecture Choices
	1. I chose .Net Core, since it is cross-platform, fast and it is the stack I am the most comfortable with.

	2. I chose to divide my solutions in 4 production projects (Core, CLI, API and Web) as well as two test projects covering Core and API.

	3. In order to make the ScanAgent fully testable, I decided to add some extra complexities to the project by adding IO adapter classes.
	   My reason for that is simple, ScanAgent is the core of the system, so it must be as stable as possible and one way of achieving it, is by making it testable.
   
    4. In order to make the ScanAgent as fast as possible, I decided to add some extra complexities with the used of Multi-Threading. 
	   I added comments in my code to explain the parallelism and thread-safety mechanism
	

Missing parts to be production ready
	1. I would have embedded and APM Agent to automatically measure the performance of my application and track errors. For example : https://www.elastic.co/guide/en/apm/agent/dotnet/current/intro.html#how-it-works

	2. I would have conterized my app with Docker for easier deployment and scalability.

	3. I would have tested all my classes. For this POC, I only tested the core of the system and not the end user interface (CLI and Web)
	
	4. I would have implemented a mechanism to easily translate the application user message
	
	5. I would have created a certificate and use https for my API
