How to namespace bootstrap

https://www.youtube.com/watch?v=f4fikVsxbzw

Steps:

1. Clone bootrap twice into 2 different folders

   git clone https://github.com/twbs/bootstrap.git

2. Reset the first folder to the current version 

   git reset --hard dca1ab7d877bc4b664b43604657a2b5fbe2b4ecb

3. Reset the second folder to the new version 

   git reset --hard <new hash>  (replace with the new hash)

4. Replace the 2 files in BOTH scss folders with the ones in the patch-src folder:

_variables.scss
bootstrap.scss

5. Adjust the changes in the new version to match the old version by discarding lines in sourcetree. 

6. Copy the updated files back into the patch-src folder:

_variables.scss
bootstrap.scss

7.  Run the build (see https://getbootstrap.com/docs/4.3/getting-started/build-tools/)

npm install 
npm rebuild node-sass
npm run dist

8.  Copy these 2 files from the dist/css folder of the bootsrap repo into WorkBC.Web\wwwroot\bootstrap

bootstrap.css
bootstrap.min.css

9.  Update step 2 in this document to put the new hash in the old hash location.
