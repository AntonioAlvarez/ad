using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		Configuration configuration=new Configuration();
		configuration.Configure();
		
		configuration.AddAssembly(typeof(Categoria).Assembly);
		//new SchemaExport(configurations).Execute(true, false, false);
		
		ISessionFactory sessionFactory = configuration.BuildSessionFactory();
		
		ISession session = sessionFactory.OpenSession();
		Categoria categoria = (Categoria)session.Load(typeof(categoria),2L);
		Console.WriteLine ("Categoria Id={0} Nombre={1}", categoria.Id, categoria.Nombre);
		categoria.Nombre = DataTime.Now.ToString();
		session.SaveOrUpdate(categoria);

		session.close();
		sessionFactory.close();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
