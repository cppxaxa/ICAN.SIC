* [x] Make add directory separator to make Linux mono friendly
* [x] Support for extending the application with python plugins
* [ ] Document the python based plugin development

# ICAN.SIC

* SIC -> Second In Command
* The project is based on Publisher-Subscriber architecture written in C#.
* The project identifies it's dll modules and loads them.
* The plugins can interact with each other without knowing each other's existence, hence they are loosely coupled.
* Each plugin can be developed separately.

Some of the first plugins are, SIML based chatbot and a web based chatInterface.

# Code Documentation

https://github.com/cppxaxa/ICAN.SIC/blob/master/Documentation.md

# Executables

https://github.com/cppxaxa/ICAN.SIC_Executables

# Special Features

* [x] Anomaly detection in live camera feed
* [x] Describe the image on live camera feed
* [x] List the objects present in the live camera feed
* [x] Generate alerts on visuals if requested
* [x] NLP based responses from knowledge base
* [ ] Dynamically add data to knowledge base
* [ ] Respond to geo-requests (missing impl for linux)

# Special instructions to fit image processing stack

1. Put the plugin "https://github.com/cppxaxa/ICAN.SIC.Plugin.ICANSEE"
2. Separately run the python execution server "https://github.com/cppxaxa/PyTaskExecuterService"
3. Atlast some introductory videos "https://www.youtube.com/watch?v=Jbekkk1N-Jw", "https://www.youtube.com/watch?v=VBhKXbQ_35E"

