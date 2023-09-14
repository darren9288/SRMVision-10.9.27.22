@echo off
set datetimef=%date:~-4%_%date:~-7,2%_%date:~-10,2%__%time:~0,2%_%time:~3,2%_%time:~6,2%


MKDIR D:\OLD_SVG\SVG_%datetimef%\

copy AlarmHistory.dll		D:\OLD_SVG\SVG_%datetimef%\ /y

copy AutoMode.dll		D:\OLD_SVG\SVG_%datetimef%\ /y

copy Common.dll			D:\OLD_SVG\SVG_%datetimef%\ /y

copy ImageAcquisition.dll	D:\OLD_SVG\SVG_%datetimef%\ /y

copy IOMode.dll			D:\OLD_SVG\SVG_%datetimef%\ /y

copy Lighting.dll		D:\OLD_SVG\SVG_%datetimef%\ /y

copy SharedMemory.dll		D:\OLD_SVG\SVG_%datetimef%\ /y

copy User.dll			D:\OLD_SVG\SVG_%datetimef%\ /y

copy VisionModule.dll		D:\OLD_SVG\SVG_%datetimef%\ /y

copy VisionProcessForm.dll	D:\OLD_SVG\SVG_%datetimef%\ /y

copy VisionProcessing.dll	D:\OLD_SVG\SVG_%datetimef%\ /y

copy SRMVision			D:\OLD_SVG\SVG_%datetimef%\ /y




MKDIR D:\OLD_SVG\SVG_%datetimef%\zh-CHS\

copy zh-CHS\*.*		D:\OLD_SVG\SVG_%datetimef%\zh-CHS\ /y

MKDIR D:\OLD_SVG\SVG_%datetimef%\zh-CHT\

copy zh-CHT\*.*		D:\OLD_SVG\SVG_%datetimef%\zh-CHT\ /y


echo.
echo.
echo.
echo.
echo.
echo.
echo ************ Backup to D:\OLD_SVG\SVG_%datetimef%\ done! *************
echo.
echo.
echo.
echo.
echo.
PAUSE

