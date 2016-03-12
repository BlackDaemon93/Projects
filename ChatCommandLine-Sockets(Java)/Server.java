
import java.io.*;
import java.net.*;
import java.util.*;

/**
 *
 * @author BlackDaemon
 */

public class Server {
    private static int ID; 
    private ArrayList<ClientThread> al; //arraylist pt utilizatori
    static int port;
    static String serverAddress;
    private boolean keepGoing;
    
    Socket socket;
    ObjectInputStream sInput;
    ObjectOutputStream sOutput;
    
    public Server(int port){
        this.port = port;
        al = new ArrayList<ClientThread>();
    }
    
    public void start(){
        keepGoing = true;
        try{
            ServerSocket serverSocket = new ServerSocket(port);
            System.out.println("Waiting for clients to connect on: " + serverAddress+ ":"+ port);
            
            while(keepGoing){
                
                Socket socket = serverSocket.accept(); // accepta conexiune
                if(!keepGoing) break;
                ClientThread c = new ClientThread(socket);
                al.add(c);
                c.start();
            }
            
            try{
                serverSocket.close();
                for(int i=0;i<al.size();i++){
                    ClientThread tc = al.get(i);
                    try{
                        tc.sInput.close();
                        tc.sOutput.close();
                        tc.socket.close();
                    }
                    catch(IOException ioE){
                        
                    }
                }
            }
             catch(Exception e){
                 System.out.println("Exception: shutdown the server and clients" + e);
             }   
            
        }
        catch (IOException e) {
            System.out.println("Exception new ServerSocket\n");
        }
    }
    
     private synchronized void Bcast(String message) {
         String messageLn = message + "\n";
         
         for (int i = al.size(); --i >= 0;){
             ClientThread ct = al.get(i);
             if (!ct.writeMsg(messageLn)){
                 al.remove(i);
                 System.out.println(ct.username + " disconnected.");
             }
         }
    }
     
     private synchronized void MSG(String username,String message){
         String messageLn = message + "\n";
         for (int i = al.size(); --i>=0;){
             ClientThread ct = al.get(i);
             if (ct.username.equals(username)){
                 writeMsg(ct.username + ":" + messageLn);
             }
         }
     }
    
     synchronized void remove(int id) {
       for(int i = 0; i < al.size(); ++i) {
           ClientThread ct = al.get(i);
            if(ct.id == id) {
                al.remove(i);
                return;
            }
       }
     }
     
     private boolean writeMsg(String msg) {
         
         try {
             sOutput.writeObject(msg);
         }
         catch(IOException e) {
             System.out.println("Sending error");
             System.out.println(e.toString());
         }
         return true;
     }
     
   public static void main(String[] args) {
       
       Scanner sc = new Scanner(System.in);
       System.out.print("Server Address : ");
       serverAddress = sc.nextLine();
       System.out.print("Server Port : ");
       port = sc.nextInt();
       Server server = new Server(port);
       System.out.println("Ready!");
       server.start();
       
   }

   class ClientThread extends Thread {
           Socket socket;
           ObjectInputStream sInput;
           ObjectOutputStream sOutput;
           int id;
           String username;
           String nick;
           ChatMessage cm;
           
    
           Scanner scan = new Scanner(System.in);
           
           ClientThread(Socket socket){
                id = ++ID;
                this.socket = socket;
                
                try{
                    sOutput = new ObjectOutputStream(socket.getOutputStream());
	            sInput  = new ObjectInputStream(socket.getInputStream());
                    
                    username = (String) sInput.readObject();
                    
                    System.out.println(username + " connected.");
                }
                catch (IOException e){
                    System.out.println("Exception I/O Streams: " + e);
                    return;
                }
                 catch (ClassNotFoundException e) {
	        }
           }
           
           public void run(){
               boolean keepGoing = true;
               while(keepGoing){
                   try{
                       cm = (ChatMessage) sInput.readObject();
                   }
                   catch(IOException e){
                       System.out.println(username + " Exception reading Streams: " + e);
                       break;
                   }
                   catch(ClassNotFoundException e2) { break; }
                   
                   String message = cm.getMessage();
                   
                   switch(cm.getType()){
                       
                       case ChatMessage.QUIT: 
                           keepGoing = false;
                           System.out.println(username + " disconnected");
                           break;
                       case ChatMessage.LIST:
                           writeMsg("Users: ");
                           for (int i = 0; i < al.size(); ++i){
                               ClientThread ct = al.get(i);
                               writeMsg((i+1)+")" + ct.username);
                           }
                           break;
                       case ChatMessage.BCAST:
                           Bcast(username + ": " + message);
                           break; 
                       case ChatMessage.MSG :
                           String user = cm.getUsername();
                           MSG(user,message);
                           break;
                       case ChatMessage.NICK :
                           user = cm.getUsername();
                           NICK(user,nick);
                           break;
                       default : 
                           writeMsg("Bad Input");
                           break;
                   }
               }
               remove(id);
               close();
           }
           
           private synchronized void NICK(String username,String nick){
               
               for (int i = al.size(); --i>=0;){
                   ClientThread ct = al.get(i);
                   if (ct.username.equals(username)){
                       //ct.username.replaceAll(username,nick);
                   }
               }
           }
           
           private void close() {
               try {
                   if(sOutput != null) sOutput.close();
               }
               catch(Exception e) {}
               try {
                   if(sInput != null) sInput.close();
               }
               catch(Exception e) {};
               try {
                   if(socket != null) socket.close();
               }
               catch (Exception e) {}
           }
           
            private boolean writeMsg(String msg) {
                if(!socket.isConnected()) {
                    close();
                    return false;
                }
                try {
                    sOutput.writeObject(msg);
                }
                catch(IOException e) {
	                System.out.println("Sending error to " + username);
	                System.out.println(e.toString());
	            }
	            return true;
        }
   }
}