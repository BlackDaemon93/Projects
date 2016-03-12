package rmi_srv;
import java.rmi.Remote;
import java.rmi.RemoteException;

public interface I extends Remote {
    
    public void adunare(int i) throws RemoteException;
    public void scadere(int i) throws RemoteException;
    public void inmultire(int i) throws RemoteException;
    public int curent() throws RemoteException;
    
}
