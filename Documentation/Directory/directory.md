# Infantry Directory Service Protocol

Default port 4850

## Wireshark
Filter is udp.port == 4850.

## Challenge Packet

### Request (Client -> Server)
Length: 8

|Data Type|Name|Value|Notes|
|-|-|-|-|
|WORD|ID|0x0001|Packet ID|
|BYTE|Unknown||seen 0x02 old, 0x03 new|
|BYTE|Unknown||seen 0x00|
|BYTE[4]|Challenge||4 random numbers|

### Response (Server -> Client)
Length: 8

|Data Type	|Name			|Value		|Notes			|
|-----------|---------------|-----------|---------------|
|WORD		|ID				|0x0002		|Packet ID		|
|BYTE     	|Unknown1		|0x42		|     			|
|BYTE     	|Unknown2		|0x0c		|				|
|BYTE[4]  	|Challenge		|			|4 random numbers from the request|

## Client Info

### Request (C->S)
Length: 41

|Data Type	|Name			|Value		|Notes			|
|-----------|---------------|-----------|---------------|
|WORD		|ID				|0x0003		|Packet ID		|
|BYTE[6]	|Unknown		|			|Seen all zeros |
|BYTE		|Unknown		|			|Seen 0x01		|
|CHAR[9]	|Client Name?	|			|Seen "Infantry"|
|WORD		|Unknown		|			|Seen 0xa00f	|
|DWORD		|Unknown		|			|Changes per request|
|BYTE		|Unknown		|			|Seen 0x00		|
|WORD		|Unknown		|			|Changes per request|
|BYTE[14]	|Unknown		|			|Seen 00 80 a7 8f 77 51 da 42 00 64 f4 19 00 00|


## Zonelist
This queries information about the currently listed zones.

### Sequence

### Zone List Request (Client -> Server)
Length: 28

|Data Type	|Name			|Value		|Notes			|
|-----------|---------------|-----------|---------------|
|WORD     	|ID				|0x0005		|Packet ID		|
|BYTE[26]	|Unknown		|			|				|

### Zone List Response (Server -> Client)

The length is variable and depends on the number of zones. If the number of zones makes this packet larger than 512 it will be split into multiple packets.

|Data Type	|Length	|Name			|Value		|Notes				|
|-----------|-------|---------------|-----------|-------------------|
|WORD     	|2		|ID				|0x0003		|Packet ID			|
|WORD		|2		|Unknown		|			|Seen 0x00000000	|
|BYTE		|1		|ChunkNumber	|			|Which chunk it is in the sequence. Starts at 0.|
|BYTE[4]	|4		|Unknown		|			|Seen all zeros		|
|BYTE		|1		|Unknown		|			|Seen 0x08			|
|BYTE[2]	|2		|Unknown		|			|Seen all zeros		|
|WORD		|2		|ChunkLength	|			|Total size of the chunked data in little endian format.|
|BYTE[2]	|2		|Unknown		|			|					|

The zone data starts with a single byte, 0x01. Then the following is repeated for each zone:

|Data Type	|Length	|Name			|Value		|Notes							|
|-----------|-------|---------------|-----------|-------------------------------|
|DWORD     	|4		|ServerIP		|			|IP Address of the zone server.	|
|WORD		|2		|ServerPort		|			|Port for the zone server.		|
|BYTE[6]	|6		|Unknown		|			|Seen 0x000001009B00			|
|CHAR[32]	|32		|ZoneName		|			|Name of the zone.				|
|BYTE		|1		|Unknown		|			|Seen 0x32						|
|BYTE		|1		|Unknown		|			|Seen 0x00						|
|BYTE		|1		|IsAdvanced		|			|Boolean, 1 = true, 0 = false	|
|BYTE[29]	|29		|Unknown		|			|Seen all zeros.				|
|CHAR[*]	|*		|ZoneDescription|			|Variable length null terminated string that describes the zone|

#### IP Address format
Each byte is one part of an IP address from left to right. So
108.61.133.122 is represented as the DWORD 0x6C3D857A.

### Acknowledge Zone Packet
Length: 8

|Data Type	|Length	|Name			|Value		|Notes				|
|-----------|-------|---------------|-----------|-------------------|
|WORD     	|2		|ID				|0x000B		|Packet ID			|
|WORD		|2		|Unknown		|			|Seen 0x0000		|
|BYTE		|1		|ChunkNumber	|			|Which chunk in the sequence that was acknowledged. Starts at 0.|
|BYTE[3]	|3		|Unknown		|			|Seen all zeros		|

## Disconnect Request

|Data Type	|Length	|Name			|Value		|Notes				|
|-----------|-------|---------------|-----------|-------------------|
|WORD     	|2		|ID				|0x0007		|Packet ID			|
|WORD     	|2		|Unknown		|			|Packet ID			|
|DWORD     	|2		|Challenge		|			|Challenge token sent with initial 0x0001 packet.|
