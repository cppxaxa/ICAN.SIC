# ICAN.SIC Executables

The source is available in my github.

# Getting started

Just run the BrokerHub.Host.exe

It loads plugins and reports any error in console.

# Description

SIC -> Second In Command
The project is based on Publisher-Subscriber architecture written in C#.
The project identifies it's dll modules and loads them.
The plugins can interact with each other without knowing each other's existence, hence they are loosely coupled.
Each plugin can be developed separately.

Some of the first plugins are, SIML based chatbot and a web based chatInterface.