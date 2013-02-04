using System;
using Gtk;
using Npgsql;//para escribir los nombres cortos de las clases
using System.Data;


public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnExecuteActionActivated (object sender, System.EventArgs e)
	{
		NpgsqlConnection/*esta es la interfaz*/ dbConnection = new NpgsqlConnection ("Server=localhost;Database=aula;User Id=dbprueba;Password=root");
		NpgsqlCommand selectCommand = dbConnection.CreateCommand();/*nos permite crear un comando*/
		selectCommand.CommandText = "select * from articulo";
		IDbDataAdapter/*esta interfaz es la que implementan luego las clases DataAdapter concretas*/
			dbDataAdapter = new NpgsqlDataAdapter();
				new NpgsqlCommandBuilder((NpgsqlDataAdapter)dbDataAdapter);/*nos dan 
			varias alternativas para construir el dataAdapter
			[todas requieren de objetos concretos ,no nos valen interfaces]*/
		
		dbDataAdapter.SelectCommand = selectCommand;
			/*para establecer el selectCommand si que deja establecerlos como una interfaz(IdbCommand)*/
		DataSet dataSet = new DataSet();/*podemos indicar 
		un nombre del DataSet,pero lo vamos a dejar sin rellenar
		//el dataSet se puede rellenar de varias 
		maneras pero lo vamos a rellenar con
		un DataAdapter(este necesita una cadena
       de conexion o una conexion)*/
		 
		dbDataAdapter.Fill(dataSet);/*el dataSet tendra que ser rellenado con el contenido
		de dbDataAdapter// el dataAdapter ya configurado rellena el dataSet*/
		
		Console.WriteLine("Tables.Count={0}",dataSet.Tables.Count);
		foreach(DataTable dataTable in dataSet.Tables) 
			show(dataTable);
		
		DataRow dataRow = dataSet.Tables[0].Rows[0];
		dataRow ["nombre"]= DateTime.Now.ToString();
		Console.WriteLine("Tabla con los cambios");
		show (dataSet.Tables[0]);
		
		((NpgsqlDataAdapter)dbDataAdapter).RowUpdating += delegate(object dbDataAdapterSender, NpgsqlRowUpdatingEventArgs eventArgs){
		     Console.WriteLine ("RowUpdating command.CommandText={0}",eventArgs.Command.CommandText);
		                                                           
		     foreach(IDataParameter dataParameter in eventArgs.Command.Parameters)
		     Console.WriteLine("{0}={1}", dataParameter.ParameterName,dataParameter.Value);
		
		};
		
		dbDataAdapter.Update (dataSet);
	}
	
	private void show(DataTable dataTable)
	{
		/*para que una coleccion pueda 
		 * formar parte de un foreach 
		 * tiene que implementar la interfaz 
		 * enumereable(solo tiene el metodo getEnumerator();)
		 * el cual permite recorrer de uno en uno*/
		
		//primero recorro todas las columnas de la tabla y las sacamos por consola
		foreach (DataColumn dataColumn in dataTable.Columns)
			Console.WriteLine("Column.Name={0}",dataColumn.ColumnName);
		
		//
		foreach (DataRow dataRow in dataTable.Rows){
			foreach (DataColumn dataColumn in dataTable.Columns)
				Console.Write ("[{0}={1}] ",dataColumn.ColumnName, dataRow[dataColumn]);
			Console.WriteLine();
		}
			/*para acceder al dataRow una opcion es 
			indicar el indice(un indice numerico o column name)-->dataRow ["id"]
			*/
			
			/*si quiero hacerlo generalizado coger el de todas las columnas*/
	}
}


/*EJEMPLO DE INTERNET
 * 
 * Posiblemente, leer hacia delante sea una opción muy interesante en 
 * algunas ocasiones… pero en otras muchas no lo será y tendremos que 
 * recurrir a los versátiles DataSet. Haremos un uso básico de los 
 * mismos en este ejemplo, porque como os he dicho ya sus posibilidades
 * son realmente amplias. Insertaremos un nuevo concepto, el de la
 * clase MySqlAdapter que será quien se encargue de rellenar el DataSet
 * a través del comando que ejecute el SELECT.
  Previamente deberemos incluir la referencia a nuestro proyecto para 
  el ensamblado System.Data que lo encontraréis dentro del menú de 
  referencias en la pestaña “Paquetes”. En el comienzo del fichero 
  también deberéis rellenar su respectivo using System.Data.
  Una vez hecho esto, para rellenar el DataSet sólo tendremos que hacer:


public static void Main(string[] args) 
{ 
	MySqlConnection conexion = new MySqlConnection(); 
	conexion.ConnectionString = "database=linuxhispano;
server=localhost;user id=root; pwd=contraseña"; 
	conexion.Open(); 

	// nuevos objetos a utilizar 
	DataSet datos = new DataSet(); 
	MySqlDataAdapter adaptador = new MySqlDataAdapter(); 

	try 
	{ 
		MySqlCommand comando = new MySqlCommand
("SELECT * FROM ejemplo", conexion); 
		adaptador.SelectCommand = comando; 
		adaptador.Fill(datos); 
	} 
	catch(MySqlException exc) 
	{ 
		Console.WriteLine("Ocurrió un error : " + exc.Message); 
	} 
	finally 
	{ 
		conexion.Close();				 
	} 
}

Dentro de datos, tendremos un DataTable con todos los datos. Pero, ¿cómo acceder a ellos? Espero que con el siguiente bucle que voy a mostrar se quede todo un poco más claro. En este bucle recogeremos la tabla del DataSet y posteriormente la recorreremos primero por filas y luego cada fila por elementos. Imprimiremos el resultado por pantalla. Para no repetir código sólo pondremos la parte nueva y dónde va insertada.

[...]
// nuevos objetos a utilizar 

DataSet datos = new DataSet(); 
MySqlDataAdapter adaptador = new MySqlDataAdapter();
[...]
	adaptador.Fill(datos); 
	DataTable tabla = datos.Tables[0]; 
	int i = 0; 
	foreach(DataRow fila in tabla.Rows) 
	{ 
		Console.WriteLine("Fila " + i.ToString()
 + " --> Elementos : "); 
		int j = 0; 
		foreach(Object celda in fila.ItemArray) 
		{ 
			Console.WriteLine("\tElemento : "
 + j.ToString() + " - " +  celda.ToString()); 
			j++; 
		} 
		i++; 
	}				 
} 
catch(MySqlException exc)
[...]


La salida sería algo así:

Fila 0 --> Elementos : 
	Elemento : 0 - 25 
	Elemento : 1 - Gráficos 
Fila 1 --> Elementos : 
	Elemento : 0 - 26 
	Elemento : 1 - Programación 
Fila 2 --> Elementos : 
	Elemento : 0 - 27 
	Elemento : 1 – Sonido
A partir de aquí, podríamos modificar los registros y no almacenarlos luego en la base de datos, convertir la salida en un fichero XML de manera directa, realizar un mapeo de la base de datos y hacer que los cambios sean instantáneos en ella… en definitiva, todo un mundo de opciones a muy pocas líneas de código de distancia que posibilitan, como he dicho desde un comienzo, un desarrollo del código ligero y rápido en el manejo de datos.*/