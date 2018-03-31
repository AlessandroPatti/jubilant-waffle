<h1 align="center"> Jubilant Waffle </h1>
<h2 align="center"> Programmazione di sistema </h2>
<h6 align="center"> Anno accademico 2016-2017 </h6>


### Project
#### File sharing over LAN
Goal of the project is the implementation of a solution allowing users to send files and folders among computers using the LAN.
### Requirements
The service, running in background, announces itself to all other hosts in the network. The network discovery can exploit mechanisms based on multicast UDP packets or similar technologies.
A host can be configured for private mode: in this case the service will not announce its presence to other hosts and won't accept any file, but will be able to send them anyway.

If the user of an host running the application right-clicksr a file or a folder in the explore, he will see an entry allowing him to share the file or the folder to all the hosts online or to a subset of them.
There are no requirements on the protocol used for the communications.

An host may be configured to wait for explicit confirmation from the user upon receiving a new request for a file or to accept it automatically. Moreover it should be possible to set a default folder for incoming files as well as asking the destination folder for each request. Each host stores the identity of the user and uses it to announce itself on the network. The overall behaviour of the application should be configurable through the application settings window.

A host must be able to accept multiple incoming file at the same time.

The application should also manage any case of multiple incoming file with the same file name to be stored in the same folder by proper renaming.

The host sending a file must display a progress bar, an estimation for the transfer time and give the user the possibility to cancel the operation.
