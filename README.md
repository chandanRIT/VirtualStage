VirtualStage (Project for the course Applications in Virtual Reality)
===
It is about controlling an avatar (main character) in a virtual world using Kinect SDK and syncing its pose and movements across multiple Unity clients (audiences). A Kinect WPF application receives raw Kinect data of a human body standing in front of the Kinect, processes it using the Kinect SDK libraries and then communicates it to a Unity server instance to control the Avatar in the Unity world. All the Unity clients sync the position of the avatar from the server. One of the Unity clients acts as the Avatar and the rest are all Audience in the virtual world. 

Setting up the workstation and Running the project:
---
Please refer to the Project report in the pdf. It provides detailed info on how to setup and extend the project.

For setting up the machines, please ensure that they meet the minimum requirements as stated in the Input/Output section of the Project Documentation PDF.

For Developers and Testers of the System: 
---
(Assumes running the client and server Unity instances on the same machine. The Clients-Server Unity architecture will easily extend to multiple machines. Visual Studio’s WPF application is also on the same machine.)

1. The Computer should have the minimum requirements as stated in Input/Output section. 
2. Start at least two Unity instances each pointing to two different copies of the original project. One will serve as the server and the other will serve as the client.
3. To configure one of the projects as the server, checkmark Server script in Inspector view of the Main Camera of the scene to launch this project scene with Server configuration. Make sure the Client script is unchecked.
4. To configure the other project as the client, checkmark Client Script in the inspector view of the Main Camera. Make sure the Server script is unchecked.
5. Then the respective projects can be launched by selecting the scene called ‘stage’ and hitting play button in Unity.
6. The two main components in the Unity project are ClientScript.cs and ServerScript.cs. ClientScript code is located in ClientScript.cs, it handles the client UI and listening on UDP port from Kinect WPF application and updating the main character’s position. ServerScript.cs code which has the UI and handles starting of the server. It also makes the RPC client calls to sync across clients. NetworkingScript.cs is responsible for spawning, removing of main character and audiences. It also does syncing across multiple clients. RotateCamera.cs is used in viewpoint modification.
7. Now moving to Kinect WPF application, it just has one main file which is MainWindow.xaml.cs. This file is an extension of the BodyBasics-WPF sample code included in the WIndows Kinect SDK. The changes we made to the code are between these comments: //VS-START *** and //VS-END. We have thoroughly documented this section in the code and it should be pretty straight forward for anyone who wishes to extend the application.
