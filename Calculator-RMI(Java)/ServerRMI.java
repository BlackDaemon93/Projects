package rmi_srv;
import java.util.*;
import java.rmi.registry.*;

public class ServerRMI {

    public static void main(String[] args) throws Exception {
    
        Scanner sc = new Scanner(System.in);
        System.out.print("Portul: "); int port = sc.nextInt();
        C Ob = new C(0);
        Registry reg = LocateRegistry.createRegistry(port);
        reg.bind("Ob",Ob);
        System.out.println("Serverul a pornit");
    }  
}
