
import java.io.Serializable;

/**
 *
 * @author BlackDaemon
 */
public class ChatMessage implements Serializable{
    static final int LIST = 0; // utilizatorii
    static final int BCAST = 1; //mesaj catre toti utilizatori
    static final int QUIT = 2; //deconectarea unui utilizator de la server
    static final int MSG = 3; //mesaj catre un uitilizator
    static final int NICK = 4; //anunta conectarea la sv a unui utilizator
    int type;
    String message;
    String username;
    
    //constructor
    ChatMessage(int type,String username,String message){
        this.type = type;
        this.username = username;
        this.message = message;
        
    }
    
    String getUsername(){
        return username;
    }
    
    int getType(){
        return type;
    }
    
    String getMessage(){
        return message;
    }
}
