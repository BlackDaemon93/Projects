
import java.net.*;
import java.io.*;
import java.util.*;

/**
 *
 * @author BlackDaemon
 */


public class Client {
    
    private ObjectInputStream sInput;
    private ObjectOutputStream sOutput;
    private Socket socket;
    
    static private String server,username;
    static private int port;
    
    class GetServer extends Thread{
        
        public void run(){
            while(true){
                try{
                    String msg = (String) sInput.readObject();
                    System.out.println(msg);
                }
                catch(IOException io){
                    System.out.println("Server has close the connection.");
                    break;
                }
                catch(ClassNotFoundException e){
                    
                }
            }
        }
    }
    
    Client(String server,int port, String username){
        this.server = server;
        this.port = port;
        this.username = username;
    }
    
    public boolean start(){
        try{
            socket = new Socket(server,port);
        }
        catch(Exception e){
            System.out.println("Error connecting to the server: " + e);
            return false;
        }
        
         System.out.println("Connection accepted " + socket.getInetAddress() + ":" + socket.getPort());
         
         try{
             sInput = new ObjectInputStream(socket.getInputStream());
             sOutput = new ObjectOutputStream(socket.getOutputStream());
         }
         catch(IOException io){
             System.out.println("Exception I/O Streams: " + io);
             return false;
         }
         
         new GetServer().start();
         
         try{
             sOutput.writeObject(username);
         }
         catch(IOException io){
             System.out.println("Exception: login " + io);
             disconnect();
             return false;
         }
         
         return true;
    }

    void sendMessage(ChatMessage msg){
        try{
            sOutput.writeObject(msg);
        }
        catch(IOException io){
            System.out.println("Exception writing to server: " + io);
        }
    }
    
    private void disconnect(){
        try{
            if (sInput != null) sInput.close();
        }
        catch(Exception e){}
        
        try{
            if (sOutput != null) sOutput.close();
        }
        catch(Exception e){}
        
        try{
            if (socket != null) socket.close();
        }
        catch(Exception e) {}
    }
    
    public static void main(String[] args){
        
        Scanner scan = new Scanner(System.in);
        System.out.print("Set Server: ");
        String serverAddress = scan.nextLine();
        System.out.print("Set Port: ");
        int portAddress = scan.nextInt();
        System.out.print("Set Username: ");
        String userName = scan.next();
        

        Client client = new Client(serverAddress,portAddress,userName);
        
        if (!client.start()) return; // testam daca ne putem conecta la server
        
        
        while (true) {
             String c = scan.next();
             String user;
             String msg;
             
             if (c.equalsIgnoreCase("QUIT")){
                 client.sendMessage(new ChatMessage(ChatMessage.QUIT,"",""));
                 break;
             }
             
             else if (c.equalsIgnoreCase("LIST")){
                 client.sendMessage(new ChatMessage(ChatMessage.LIST,"",""));
             }
             else if(c.equalsIgnoreCase("MSG")){
                 System.out.print("Message to: ");
                 user = scan.next();
                 msg = scan.nextLine();
                 client.sendMessage(new ChatMessage(ChatMessage.MSG,user,msg));
             }
             else if(c.equalsIgnoreCase("BCAST")){
                 msg = scan.nextLine();
                 client.sendMessage(new ChatMessage(ChatMessage.BCAST,"",msg));
             }
             else if(c.equalsIgnoreCase("NICK")){
                 String nick = scan.nextLine();
                 client.sendMessage(new ChatMessage(ChatMessage.NICK,nick,""));
             }
             else {
                 System.out.println("Invalid Input!");
             }
             
        }
        
        client.disconnect();
    }
}
