### SelfHealing.ps1 ###
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

###
### Learning a bit about workflows and how to use them in order to resume scripts
### after rebooting
###
SelfHealingWorkFlow

WorkFlow SelfHealingWorkFlow
{
    $path = "C:\Program Files\InfoPoint\"
    $process = "TOURISTINFO"
    $maxTries = 2
    $attempt = 0
    $stamp = (Get-Date).ToString("yyyy-MM-dd")
    $logPath = "C:\Program Files\InfoPoint\crash"
    $logFileName = "C:\Program Files\InfoPoint\crash\" + $stamp + ".log"
    $timeToWait = 120

    ###
    ### Please note that for this assestment user email and credentials are in without encryption
    ## To check if mails were sent: oseramapollo@gmail.com password: _Gaia_8484!!
    ###
    $notificationEmailTo = "oseramapollo@gmail.com"

    CreateLogDirectory -logPath $logPath
        
    WriteLog -Level INFO -Message "Checking if process is running" -logfile $logFileName

     $isRunning = CheckProcess -p $process -ErrorAction SilentlyContinue
    
    if ($isRunning) {
        WriteLog -Level INFO -Message "Process is running Ok" -logfile $logFileName
        Exit
    }
    else {               
        while ($attempt -lt $maxTries -and !$isRunning) {
            $attempt++
            WriteLog -Level INFO -Message "Attempting to start process for $attempt times" -logfile $logFileName
            StartProcess -process $process -path $path
            $isRunning = CheckProcess -p $process -ErrorAction SilentlyContinue
            WriteLog -Level INFO -Message "The process will be checked again in $timeToWait seconds" -logfile $logFileName
            Start-Sleep -Seconds $timeToWait    
        }
        WriteLog -Level ERROR -Message "Error the process could not be started after $attempt attempts" -logfile $logFileName
        SendNotification -EmailTo $notificationEmailTo -Subject "Error starting process" -Body "Please check attached file to verify complete log trace." -attachment $logFileName -password "_Gaia_8484!!"
    }
}

function CheckProcess {
    Param(
        [Parameter(Mandatory = $true)]
        [string]
        $process
    )
    try {
        $result = Get-Process $process -ErrorAction SilentlyContinue
        return $result
    }
    catch {
        Write-Output "The process does not even exists- Terminating script"
    }
}

function StartProcess {
    Param(
        [Parameter(Mandatory = $true)]
        [string] $process,
        
        [Parameter(Mandatory = $true)]
        [string] $path
    )
    Start-Process -FilePath $process -WorkingDirectory $path -WindowStyle Normal    
}

function WriteLog {
    Param(
        [Parameter(Mandatory = $False)]
        [ValidateSet("INFO","WARN","ERROR","FATAL","DEBUG")]
        [String]$level = "INFO",

        [Parameter(Mandatory = $True)]
        [string] $message,
 
        [Parameter(Mandatory = $False)]
        [string] $logfile
    )

    $stamp = (Get-Date).toString("yyyy/MM/dd HH:mm:ss")
    $line = "$stamp $level $message"
    if($logfile) {
        Add-Content $logfile -Value $line
    }
    else {
        Write-Output $line
    }
}

function CreateLogDirectory {
    Param(
        [Parameter(Mandatory = $true)]
        [string] $logPath
    )
    if(![System.IO.File]::Exists($logPath)){
        [system.io.directory]::CreateDirectory($logPath)
    }
}

function SendNotification {
    Param (
        [Parameter(Mandatory = $true)]
        [String] $emailTo,

        [Parameter(Mandatory = $true)]
        [String]$subject,

        [Parameter(Mandatory = $true)]
        [String]$body,

        [Parameter(Mandatory = $false)]
        [String] $emailFrom ="oseramapollo@gmail.com",

        [Parameter(mandatory=$false)]
        [String] $attachment,

        [Parameter(mandatory=$true)]
        [String]$password
    )
      
        $SMTPServer = "smtp.gmail.com" 
        $SMTPMessage = New-Object System.Net.Mail.MailMessage($emailFrom, $emailTo, $subject, $body)
        if ($attachment -ne $null) {
            $SMTPattachment = New-Object System.Net.Mail.Attachment($attachment)
            $SMTPMessage.Attachments.Add($SMTPattachment)
        }
        $SMTPClient = New-Object Net.Mail.SmtpClient($SmtpServer, 587) 
        $SMTPClient.EnableSsl = $true 
        $SMTPClient.Credentials = New-Object System.Net.NetworkCredential($emailFrom, $Password); 
        $SMTPClient.Send($SMTPMessage)
} 

