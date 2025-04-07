// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;


// Check Text File for Saved Information



// VARIABLES

//Main Directory

string save_file;
string main_folderpath;

IList<string> monthnames = new List<string> {};
IList<string> sub_foldernames = new List<string> {};


//Folder Variables
IList<String> folder_list;
string previous_folder;
string previous_folder_date;

//Date Handling Variables
string thisdaystring;
string s_year;
string s_month;
string s_day;

//Folder Names
string thisday_foldername;
string sub_folder_path;
string sub_sub_folder_path;

//File Transfer
string start_directory;
string end_directory;
int transfer_count;

string back_directory;
int transfer_count_back;

//Folder in which the monthly storage folders are created
save_file = @"D:\Oliver SchleppThat\Eigene Fotos und Videos\Handy\Xiami Redmi Note 7\Handycleaner_Save_File" ;
//main_folderpath = @"D:\Oliver SchleppThat\Eigene Fotos und Videos\Handy\Xiami Redmi Note 7" ;

// List of Months
monthnames = new List<string> {"Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "Novemebr", "Dezember"};

//List of possible subfolder names to use
//sub_foldernames = new List<string> {"Screenshots", "Back", "Magic", "Fotos", "Vids", "Whatsapp"};

//directory out of which the files are transferred
//start_directory = @"D:\Oliver SchleppThat\Eigene Fotos und Videos\Handy\Xiami Redmi Note 7\PhoneData";



//PROGRAMM

//Read Saved Data
(main_folderpath,start_directory,sub_foldernames) = ReadDataFromTxt(save_file);


int decision = GetUserConfirmation();
Console.WriteLine(decision);

//main_folderpath = GetMainFolderPath();

folder_list = GetFolderList(main_folderpath);
previous_folder = GetPreviousFolder(folder_list);
previous_folder_date = GetPreviousFolderDate(previous_folder);

thisdaystring = GetDateToday();

// Get Date of previous folder
(s_year,s_month,s_day) = GetYearDayMonth(thisdaystring);



//Create new monthly folder name based on todays date
thisday_foldername = CreateThisDayFolderName(s_year,s_month,s_day,monthnames);

//Create new monthly folder based on todays date
sub_folder_path = CreateSubFolder(main_folderpath,thisday_foldername);
//Create the subfolder for screenshots
sub_sub_folder_path = CreateSubFolder(sub_folder_path,sub_foldernames[0]);
end_directory = sub_sub_folder_path;
//Create the subfolder "Back" and set it to the target for the file transfer
back_directory = CreateSubFolder(sub_sub_folder_path,sub_foldernames[1]);
//Create the subfolder "Magic"
CreateSubFolder(sub_sub_folder_path,sub_foldernames[2]);

Console.WriteLine("Subfolders Created");

Console.WriteLine("Start moving!");

//end_directory = sub_sub_folder_path;

transfer_count = TransferFilesBasedOnDate(start_directory,end_directory,previous_folder_date);
Console.WriteLine("Files Transferred: " + Convert.ToString(transfer_count));


//Back Transfer
decision = GetUserConfirmation();
Console.WriteLine(decision);

transfer_count_back = TransferFilesBack(start_directory, back_directory);
Console.WriteLine(transfer_count_back);

SaveDataToTxt(thisdaystring,main_folderpath,start_directory,end_directory,sub_foldernames,back_directory,transfer_count,transfer_count_back);

// CloseApplication();




// METHODS
static int GetUserConfirmation()
{    int i = 0;
   while (i == 0)
   {
      Console.WriteLine("Start transfer?");
      Console.WriteLine("Press 'Enter' to continue or 'Esc' to close the app");
      var keyInfo = Console.ReadKey();
      Console.WriteLine("\nYou pressed: " + keyInfo.Key);

      if (keyInfo.Key == ConsoleKey.Escape)
      {
      Console.WriteLine("Closing App");
                    //Application.Exit();
      }
      if (keyInfo.Key == ConsoleKey.Enter)
      {
      Console.WriteLine("Let' get moving!");
      i++;
      }
      if (keyInfo.Key != ConsoleKey.Enter)
      {
      Console.WriteLine("Please enter one of the inputs mentioned.");
      }
   }
return i;
}


static List<string> GetFolderList(string main_folderpath)
{  IList<String> folder_list1 = new List<String>{};
   string[] subdirectories = Directory.GetDirectories(main_folderpath);
   foreach(string directory in subdirectories)
   {
   folder_list1.Add(directory);
   }
   Console.WriteLine("FolderList Created");
Console.WriteLine(folder_list1.Last());
return (List<string>)folder_list1;  
}  


static string GetPreviousFolder(IList<string> folder_list1)
{  string previous_folder = folder_list1[folder_list1.Count-3];
   Console.WriteLine("Previous Folder: "+previous_folder);
return previous_folder;  
}  


static string GetPreviousFolderDate(string previous_folder)
{  string previous_folder_date = previous_folder.Substring(71,6);
   Console.WriteLine("Previous Folder Date: "+previous_folder_date);
return previous_folder_date;  
}  


//Create a new directory for today and all nessecary subfolders
static string GetDateToday(){
DateTime thisday = DateTime.Today;
string thisdaystring = thisday.ToString("d");
Console.WriteLine("Today's date: "+thisdaystring);
return thisdaystring;
}


// Find the Year, Month and Day in the Name of the give foldername
// Substring (first number is index, second number is range)
static(string s_year,string s_month,string s_day) GetYearDayMonth(string thisdaystring){
   string s_year = thisdaystring.Substring(8);
   Console.WriteLine(s_year);
   string s_month = thisdaystring.Substring(3, 2);
   Console.WriteLine(s_month);
   string s_day = thisdaystring.Substring(0, 2);
   Console.WriteLine(s_day);
return (s_year, s_month, s_day);
}


//Create the Name for the todays folder
static string CreateThisDayFolderName(string s_year,string s_month,string s_day,IList<string> monthnames){
string thisday_foldername = s_year+s_month+s_day+" "+ monthnames[Convert.ToInt32(s_month)-1];
Console.WriteLine(thisday_foldername);
return thisday_foldername;
}


//Create a subfolder to the given folder path
static string CreateSubFolder(string target_folder_path,string sub_folder_name){
string folderpath = target_folder_path +@"\"+ sub_folder_name;
Directory.CreateDirectory(folderpath);
Console.WriteLine("New folder Created");
return folderpath;
}


// Transfer all files since the previous directory date into the newly created subfolder
// Hier Phone Folder  Path hinter das Gleichhaltszeichen !!!
static int TransferFilesBasedOnDate(string start_directory, string end_directory, string previous_folder_date){
   DirectoryInfo dir = new DirectoryInfo(start_directory);
   FileInfo[] files = dir.GetFiles();

   string first_file_to_transfer;
   bool first_file_found = false;
   int transfer_count = 0;

   foreach(FileInfo f in files){
   if(f.Name.Contains(previous_folder_date.Substring(0,2))==true && f.Name.Contains(previous_folder_date.Substring(2,2)) ==true && f.Name.Contains(previous_folder_date.Substring(4,2))== true)
   {first_file_to_transfer= f.Name ;

         if(first_file_found == false){
   Console.WriteLine("First File to transfer found");
   Console.WriteLine(first_file_to_transfer);}

   first_file_found = true;
   }

   if(first_file_found == true){
    f.MoveTo($@"{end_directory}\{f.Name}");
    transfer_count ++;
   } 
}
Console.WriteLine("File tranfer finished");
return transfer_count;
}

// write the .txt save file
static string SaveDataToTxt(string thisdaystring,string main_folderpath,string start_directory, string end_directory, IList<string> sub_foldernames, string back_directory,int transfer_count, int transfer_count_back)
{
   string file_name = "Handycleaner_Save_File";
   string path  = main_folderpath + @"\" + file_name;

         // Checks if File exists at the end of the path
        if (File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {    
                sw.WriteLine("Name Save File: " +file_name);
                sw.WriteLine("Save Date: " +thisdaystring);
                sw.WriteLine("Main Path: " +main_folderpath);
                sw.WriteLine("Start Directory: " +start_directory);
                sw.WriteLine("End Directory: " + end_directory);
                sw.WriteLine("Back Directory: " + back_directory);
                sw.WriteLine(" ");
                sw.WriteLine("File Transfered: " + transfer_count);
                sw.WriteLine("File Transfered Back: " + transfer_count_back);
                sw.WriteLine(" ");
                sw.WriteLine("Subfolder Names:");
                for (int i = 0; i < sub_foldernames.Count; i++)
                 {sw.WriteLine(sub_foldernames[i]);
                 }    


        }
      }
      Console.WriteLine("Save File Created");
   return "Done";
}


// Read Out the .txt save file
static(string main,string start,IList<string>sub) ReadDataFromTxt(string save_file){

if (File.Exists(save_file)) 
{
    Console.WriteLine("File exists. Reading starts.");


   string main = "";
   string start = "";
   IList<string> sub = new List<string>{};

   IList<int> lines_to_read = new List<int>{3,4};
   int start_sub_foldernames_list = 12;


   string path = save_file;
   var lines = File.ReadAllLines(path);

   
   int count = 0;

   foreach(var line in lines){
         count++;
         var index = line.IndexOf(":");

               if(lines_to_read[0] == count){
            index = line.IndexOf(":");
            main = line.Substring(index+2);
            Console.WriteLine(line.Substring(index+2));
               }

                if(lines_to_read[1] == count){
            index = line.IndexOf(":");
            start = line.Substring(index+2);
            Console.WriteLine(line.Substring(index+2));
               }
         
              if(count >= start_sub_foldernames_list){
             sub.Add(line);
            Console.WriteLine(line);  
       }
   }
   return (main,start,sub);
   }
   else
   { 
   string main = @"D:\Oliver SchleppThat\Eigene Fotos und Videos\Handy\Xiami Redmi Note 7";
   string start = @"D:\Oliver SchleppThat\Eigene Fotos und Videos\Handy\Xiami Redmi Note 7\PhoneData";
   IList<string> sub = new List<string> {"Screenshots", "Back", "Magic", "Fotos", "Vids", "Whatsapp"};   

        using (StreamWriter sw = File.CreateText(save_file))
            {    
                sw.WriteLine("Name Save File: FileTransferApp");
                sw.WriteLine("Save Date: Test Date");
                sw.WriteLine("Main Path: " +main);
                sw.WriteLine("Start Directory: " +start);
                sw.WriteLine("End Directory: None");
                sw.WriteLine("Back Directory: None");
                sw.WriteLine(" ");
                sw.WriteLine("File Transfered: 0");
                sw.WriteLine("File Transfered Back: 0");
                sw.WriteLine(" ");
                sw.WriteLine("Subfolder Names:");
                for (int i = 0; i < sub.Count; i++)
                 {sw.WriteLine(sub[i]);
                 }    
        }
              Console.WriteLine("Save File Created");
           return (main,start,sub);
      }
 }




// Transfer all files from one folder back to the phone
static int TransferFilesBack(string start_directory, string back_directory){
   DirectoryInfo dir = new DirectoryInfo(back_directory);
   FileInfo[] files = dir.GetFiles();

   int transfer_count = 0;

   foreach(FileInfo f in files){
   f.MoveTo($@"{start_directory}\{f.Name}");
   transfer_count ++;
   } 
Console.WriteLine("File tranfer finished");
return transfer_count;
}

/*
// Shut Down Programm
static CloseApplication(){
   Console.WriteLine("Closing Application");
}
*/

/*
string testtest = Test1();
Console.WriteLine(testtest);



static string Test1()
{ string test = "1";
return test;
 }
*/
//string main_folderpath;
//string start_directory;
//string end_directory;
/*
      Dictionary<string, string> variable_collection = new Dictionary<string, string>();
{
    variable_collection.Add(main_folderpath, value);
}

dict["Transit"].Value = "transit"
*/



/*
static string GetMainFolderPath()
{  string main_folder = main_folderpath;
   return main_folder;
}
*/