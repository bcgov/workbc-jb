SET HTTP_PROXY=http://dev.forwardproxy.aest.gov.bc.ca
SET PATH=%NVM_HOME%;%NVM_SYMLINK%;%PATH%

REM To upgrade the node version you need to RDP to the server and use nvm to 
REM install the new version.  Changing the version here without installing the new
REM one will result in an error!!!

REM TEMPORARILY COMMENTING THIS OUT BECAUSE KATARA DOENS'T HAVE NVM INSTALLED YET
REM nvm use 12.16.2
