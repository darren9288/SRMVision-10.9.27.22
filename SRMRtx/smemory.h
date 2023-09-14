//Shared Memory Settings

#ifndef __SMEMORY_H
#define __SMEMORY_H

//Shared Memory COM
typedef struct 
{
	char uCOMSendData1[50];
	char uCOMSendData2[50];
	char uCOMSendData3[50];
	char uCOMSendData4[50];
	char uCOMSendData5[50];
	char uCOMSendData6[50];
	char uCOMSendData7[50];

} SM_COM, *SMP_COM;

#endif