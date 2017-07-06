# Introduction

This is the source code of the project developed for the course "Projects and Laboratories on Communication Systems" at Politechnic University of Turin during school year 2016/2017.

# Getting started

To test this project you need:

- dev environment for board development and deployment
  - visual studio 2013 or newer
  - visit [this site](https://www.ghielectronics.com/support/netmf) for system configuration and tools
- board hardware
  - [FEZ Spider II Tinker kit](https://www.ghielectronics.com/catalog/product/543)
  - [LED Module](https://www.ghielectronics.com/catalog/product/422)
  - [Tunes module](https://www.ghielectronics.com/catalog/product/434)
  - Ethernet cable to connect to the server application
- a Windows machine to run the server with
  - visual studio for compiling the WPF project
  - apache+mysql or XAMPP to run the database and serve php pages (including phpMyAdmin)
  - static IP on a ethernet interface with IP = 192.168.1.1 in order to be reachable from the board
  - a working connection to internet in order to fetch products from upcdatabase.org

# Components

The project is composed of three parts
1. board application
2. server application
3. website + database

## Board application

This is a .NET Micro Framework project that has been developed for the FEZ II board. Connect the components as in the diagram contained in the visual studio project. Open, compile and deploy the application to the board. This sets the static IP address on the board to `192.136.1.2`.

## Database

The database is meant to be run on mySQL. From XAMPP start both mySql and apache, in order to go to [phpMyAdmin page](http://localhost/phpmyadmin) and import the [database file](Website/database/plcs.sql).

## Website
Copy the folder `website` into the `htdocs` folder of XAMPP (or in the equivalent of apache) and use a browser to use the website.

## Server application

This is a Windows Presentation Foundation project that has been developed for running on Windows machines. Compile and run the application to start listening on UDP port 8000.

# Known issues with the network

The communication between the board and the server is quite buggy. We tried different approaches:

- communications based on HTTP (rest API)
- TCP communication
- UDP communication

The problem with all those approaches is that the board is not reliable for the transmission of huge amount of data (a picture takes around 230KB) and using wireshark we detected some strange behaviour that seems to be reconducible to a bug of the framework on the board. The board does not receive at the application layer all the packets and also does not send all the data segments on the socket; we did some experiments segmenting the transmission and tagging the segments and we observed that some packets were missing when we required the board to send a lot of data together. This problem does not occurr with short transmissions.

This problem seems to exist only on FEZ spider II and not on FEZ spider board, and appears only when the board uses both the camera and the network interface.

To minimize and recover from transmission errors, we developed a transmission protocol based on Stop-and-Wait over UDP with a window of one frame.

The protocol works as follows:

- the board asks for a token to the server (a token identifies a transmission cycle)
- the server provides a random token to the board
- the board start sending the data of an image by splitting in more UDP frames, that contain both the token and a sequence number in addition to the data. The frames are sent one by one and the board waits for an ACK from the server
- the server receives the frames and sends an ACK for each one of them. In case of mismatch of sequence number a NACK is sent, asking the missing packet to the board
- the server, when the board has transmitted all the frames, sends the response
- the board receives the responses and the token lifecycle is ended

By using this protocol, the transmission time for a single image goes around 6-8 seconds. It's a deadly slow speed for this amount of data, but we could not reach better results.
