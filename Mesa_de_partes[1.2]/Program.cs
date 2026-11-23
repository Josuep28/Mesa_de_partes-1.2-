using System;
using System.IO;

namespace Mesa_de_partes_1._2_
{
    internal class Program
    {
        // ---------- Datos ----------
        static int max_sol = 100;
        static int con_sol = 0;

        // Cada solicitud usa la misma posición en todas estas listas.
        static string[] codigo_sol = new string[max_sol];
        static string[] fecha_sol = new string[max_sol];
        static string[] estado_sol = new string[max_sol];
        static string[] nombres_sol = new string[max_sol];
        static string[] tipo_id_sol = new string[max_sol];
        static string[] num_id_sol = new string[max_sol];
        static string[] telefono_sol = new string[max_sol];
        static string[] correo_sol = new string[max_sol];
        static string[] postulante_sol = new string[max_sol];
        static string[] tipo_sol = new string[max_sol];
        static string[] asunto_sol = new string[max_sol];
        static string[] nivel_sol = new string[max_sol];
        static string[] tercio_sol = new string[max_sol];
        static string[] justificacion_sol = new string[max_sol];

        static string[] tipos_doc = { "Carta de presentación", "Justificación por enfermedad", "Certificado de estudios" };
        static string[] niveles_estudio = { "Inicial", "Primaria", "Secundaria" };
        static string[] estados = { "Recibida", "En revisión", "Atendida", "Observada" };
        static string[] dominios_correo = { "gmail.com", "hotmail.com", "outlook.com", "yahoo.com" };
        static string[] criterios_orden = { "Código", "Solicitante (A-Z)", "Fecha y hora (más antigua primero)", "Estado (Recibida a Observada)" };

        static string doc_certificado = "Certificado de estudios";
        static string carpeta_recibidos = "recibidos";
        static string archivo_datos = "solicitudes.dat";

        // Se vuelve true cuando el usuario escribe 0 para cancelar.
        static bool cancelado = false;


        // ---------- Pantalla y lectura ----------
        static public void titulo()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("              MESA DE PARTES DIGITAL             ");
            Console.WriteLine("         Colegio Italiano Antonio Raimondi       ");
            Console.WriteLine("-------------------------------------------------");
        }

        // Lee una línea. Si el usuario escribe 0, marca la operación como cancelada.
        static public string leer()
        {
            string texto = Console.ReadLine();
            if (texto == null)
                texto = "";
            texto = texto.Trim();
            if (texto == "0")
                cancelado = true;
            return texto;
        }

        static public bool solo_digitos(string texto)
        {
            if (texto == "")
                return false;
            foreach (char c in texto)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        // ---------- Preguntas al usuario ----------
        // Todas dejan de preguntar si el usuario cancela (cancelado = true).
        static public string pedir_texto(string mensaje)
        {
            while (!cancelado)
            {
                Console.Write(mensaje);
                string texto = leer();
                if (cancelado)
                    break;
                if (texto != "")
                    return texto;
                Console.WriteLine("ERROR 002: ESTE CAMPO ES OBLIGATORIO.");
            }
            return "";
        }

        static public string pedir_telefono()
        {
            while (!cancelado)
            {
                Console.Write("Teléfono (9 dígitos, empieza con 9): ");
                string telefono = leer();
                if (cancelado)
                    break;
                if (telefono.Length == 9 && solo_digitos(telefono) && telefono[0] == '9')
                    return telefono;
                Console.WriteLine("ERROR 008: EL TELÉFONO DEBE TENER 9 DÍGITOS Y EMPEZAR POR 9.");
            }
            return "";
        }

        static public string pedir_tipo_id()
        {
            if (cancelado)
                return "";
            Console.WriteLine("Documento de identidad:");
            Console.WriteLine("[1] DNI");
            Console.WriteLine("[2] Carné de extranjería");
            while (!cancelado)
            {
                Console.Write("Elegir opción: ");
                string texto = leer();
                if (cancelado)
                    break;
                if (texto == "1")
                    return "DNI";
                if (texto == "2")
                    return "CE";
                Console.WriteLine("ERROR 009: ELEGIR 1 (DNI) O 2 (CARNÉ DE EXTRANJERÍA).");
            }
            return "";
        }

        static public string pedir_numero_id(string tipo_id)
        {
            int largo = 9;
            if (tipo_id == "DNI")
                largo = 8;
            while (!cancelado)
            {
                Console.Write($"{tipo_id} ({largo} dígitos): ");
                string numero = leer();
                if (cancelado)
                    break;
                if (numero.Length == largo && solo_digitos(numero))
                    return numero;
                Console.WriteLine($"ERROR 003: EL {tipo_id} DEBE TENER EXACTAMENTE {largo} DÍGITOS.");
            }
            return "";
        }

        // Muestra una lista numerada y devuelve la opción elegida.
        // palabra: "tipo", "nivel", "estado" o "criterio". codigo_error: "004", "011", "014" o "019".
        static public string pedir_opcion(string titulo_lista, string[] lista, string palabra, string codigo_error)
        {
            if (cancelado)
                return "";
            Console.WriteLine(titulo_lista);
            for (int i = 0; i < lista.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {lista[i]}");
            }
            while (!cancelado)
            {
                Console.Write("Elegir " + palabra + ": ");
                string texto = leer();
                if (cancelado)
                    break;
                int opcion;
                if (int.TryParse(texto, out opcion) && opcion >= 1 && opcion <= lista.Length)
                    return lista[opcion - 1];
                Console.WriteLine($"ERROR {codigo_error}: ELEGIR UN NÚMERO ENTRE 1 Y {lista.Length}.");
            }
            return "";
        }

        static public string pedir_tercio()
        {
            while (!cancelado)
            {
                Console.Write("¿Pertenece al tercio superior? (S/N): ");
                string texto = leer().ToUpper();
                if (cancelado)
                    break;
                if (texto == "S")
                    return "Sí";
                if (texto == "N")
                    return "No";
                Console.WriteLine("ERROR 012: RESPONDER S (SÍ) O N (NO).");
            }
            return "";
        }

        static public string pedir_correo()
        {
            while (!cancelado)
            {
                Console.Write("Correo electrónico (ej: nombre@gmail.com): ");
                string correo = leer().ToLower();
                if (cancelado)
                    break;
                if (!correo.Contains(" "))
                {
                    int pos = correo.IndexOf('@');
                    if (pos > 0 && pos == correo.LastIndexOf('@'))
                    {
                        string dominio = correo.Substring(pos + 1);
                        foreach (string valido in dominios_correo)
                        {
                            if (dominio == valido)
                                return correo;
                        }
                    }
                }
                Console.WriteLine("ERROR 010: CORREO INVÁLIDO. DOMINIOS PERMITIDOS: " + string.Join(", ", dominios_correo) + ".");
            }
            return "";
        }

        // ---------- Guardar y cargar ----------
        // Quita el símbolo | para no dañar el archivo de datos.
        static string limpiar(string texto)
        {
            return texto.Replace("|", "/");
        }

        static public void guardar_datos()
        {
            string[] lineas = new string[con_sol];
            for (int i = 0; i < con_sol; i++)
            {
                lineas[i] = limpiar(codigo_sol[i]) + "|" + limpiar(fecha_sol[i]) + "|" + limpiar(nombres_sol[i]) + "|"
                          + limpiar(tipo_id_sol[i]) + "|" + limpiar(num_id_sol[i]) + "|" + limpiar(telefono_sol[i]) + "|"
                          + limpiar(correo_sol[i]) + "|" + limpiar(postulante_sol[i]) + "|" + limpiar(tipo_sol[i]) + "|"
                          + limpiar(asunto_sol[i]) + "|" + limpiar(nivel_sol[i]) + "|" + limpiar(tercio_sol[i]) + "|"
                          + limpiar(justificacion_sol[i]) + "|" + limpiar(estado_sol[i]);
            }
            try
            {
                File.WriteAllLines(archivo_datos, lineas);
            }
            catch (IOException)
            {
                Console.WriteLine("ERROR 015: NO SE PUDO GUARDAR " + archivo_datos + ".");
            }
        }

        static public void cargar_datos()
        {
            if (!File.Exists(archivo_datos))
                return;
            try
            {
                string[] lineas = File.ReadAllLines(archivo_datos);
                foreach (string linea in lineas)
                {
                    if (con_sol >= max_sol)
                        break;
                    string[] c = linea.Split('|');
                    if (c.Length != 14)
                        continue;
                    int i = con_sol;
                    codigo_sol[i] = c[0];
                    fecha_sol[i] = c[1];
                    nombres_sol[i] = c[2];
                    tipo_id_sol[i] = c[3];
                    num_id_sol[i] = c[4];
                    telefono_sol[i] = c[5];
                    correo_sol[i] = c[6];
                    postulante_sol[i] = c[7];
                    tipo_sol[i] = c[8];
                    asunto_sol[i] = c[9];
                    nivel_sol[i] = c[10];
                    tercio_sol[i] = c[11];
                    justificacion_sol[i] = c[12];
                    estado_sol[i] = c[13];
                    con_sol++;
                }
            }
            catch (IOException)
            {
                Console.WriteLine("ERROR 016: NO SE PUDO LEER " + archivo_datos + ".");
            }
        }

        // ---------- Cargo de recepción ----------
        static public bool es_certificado(int i)
        {
            return tipo_sol[i] == doc_certificado;
        }

        // Arma el texto completo del cargo (se usa en pantalla y en el archivo).
        static public string texto_cargo(int i)
        {
            string nl = Environment.NewLine;
            string t = "=================================================" + nl;
            t += "              CARGO DE RECEPCIÓN                 " + nl;
            t += "         Colegio Italiano Antonio Raimondi       " + nl;
            t += "=================================================" + nl;
            t += $"Código       : {codigo_sol[i]}" + nl;
            t += $"Fecha y hora : {fecha_sol[i]}" + nl;
            t += $"Estado       : {estado_sol[i]}" + nl;
            t += "-------------------------------------------------" + nl;
            t += $"Postulante   : {postulante_sol[i]}" + nl;
            t += $"Documento    : {tipo_sol[i]}" + nl;
            t += $"Asunto       : {asunto_sol[i]}" + nl;
            if (es_certificado(i))
            {
                t += $"Nivel        : {nivel_sol[i]}" + nl;
                t += $"Tercio sup.  : {tercio_sol[i]}" + nl;
                t += $"Justificación: {justificacion_sol[i]}" + nl;
            }
            t += "-------------------------------------------------" + nl;
            t += $"Presentado por: {nombres_sol[i]} ({tipo_id_sol[i]}: {num_id_sol[i]})" + nl;
            t += $"Teléfono     : {telefono_sol[i]}" + nl;
            t += $"Correo       : {correo_sol[i]}" + nl;
            t += "=================================================";
            return t;
        }

        static public void mostrar_cargo(int i)
        {
            Console.WriteLine();
            Console.WriteLine(texto_cargo(i));
        }

        static public void guardar_cargo(int i)
        {
            Directory.CreateDirectory(carpeta_recibidos);
            string ruta = Path.Combine(carpeta_recibidos, codigo_sol[i] + ".txt");
            File.WriteAllText(ruta, texto_cargo(i));
            Console.WriteLine("Cargo guardado en: " + Path.GetFullPath(ruta));
        }

        // ---------- Búsquedas ----------
        // Devuelve la posición de una solicitud igual (mismo documento, postulante y tipo)
        // a la que se está registrando en la posición n. Devuelve -1 si no hay ninguna.
        static public int buscar_duplicado(int n)
        {
            for (int i = 0; i < con_sol; i++)
            {
                if (num_id_sol[i] == num_id_sol[n]
                    && tipo_sol[i] == tipo_sol[n]
                    && postulante_sol[i].ToLower() == postulante_sol[n].ToLower())
                    return i;
            }
            return -1;
        }

        static public int buscar_codigo(string codigo)
        {
            for (int i = 0; i < con_sol; i++)
            {
                if (codigo_sol[i].ToLower() == codigo.ToLower())
                    return i;
            }
            return -1;
        }

        static public bool contiene(string texto, string termino)
        {
            return texto.ToLower().Contains(termino.ToLower());
        }

        // ---------- Ordenamiento ----------
        // Intercambia dos posiciones de un arreglo.
        static void intercambiar_texto(string[] v, int a, int b)
        {
            string aux = v[a];
            v[a] = v[b];
            v[b] = aux;
        }

        // Intercambia la solicitud a con la b en los 14 arreglos paralelos,
        // para que cada solicitud siga completa en una sola posición.
        static public void intercambiar(int a, int b)
        {
            intercambiar_texto(codigo_sol, a, b);
            intercambiar_texto(fecha_sol, a, b);
            intercambiar_texto(estado_sol, a, b);
            intercambiar_texto(nombres_sol, a, b);
            intercambiar_texto(tipo_id_sol, a, b);
            intercambiar_texto(num_id_sol, a, b);
            intercambiar_texto(telefono_sol, a, b);
            intercambiar_texto(correo_sol, a, b);
            intercambiar_texto(postulante_sol, a, b);
            intercambiar_texto(tipo_sol, a, b);
            intercambiar_texto(asunto_sol, a, b);
            intercambiar_texto(nivel_sol, a, b);
            intercambiar_texto(tercio_sol, a, b);
            intercambiar_texto(justificacion_sol, a, b);
        }

        // Devuelve el texto por el que se compara la solicitud i según el criterio (1 a 4).
        static public string clave_orden(int i, int criterio)
        {
            if (criterio == 1)
                return codigo_sol[i];
            if (criterio == 2)
                return nombres_sol[i].ToLower();
            if (criterio == 3)
            {
                // "dd/MM/yyyy HH:mm" se reordena como "yyyyMMddHHmm" para comparar como texto.
                string f = fecha_sol[i];
                return f.Substring(6, 4) + f.Substring(3, 2) + f.Substring(0, 2)
                     + f.Substring(11, 2) + f.Substring(14, 2);
            }
            return Array.IndexOf(estados, estado_sol[i]).ToString("00");
        }

        // Compara las solicitudes a y b. Si empatan, desempata por código.
        static public int comparar(int a, int b, int criterio)
        {
            int r = string.CompareOrdinal(clave_orden(a, criterio), clave_orden(b, criterio));
            if (r == 0)
                r = string.CompareOrdinal(codigo_sol[a], codigo_sol[b]);
            return r;
        }

        // Ordenamiento por selección: en cada vuelta busca la solicitud menor
        // entre i y el final, y la coloca en la posición i.
        static public void ordenar(int criterio)
        {
            for (int i = 0; i < con_sol - 1; i++)
            {
                int menor = i;
                for (int j = i + 1; j < con_sol; j++)
                {
                    if (comparar(j, menor, criterio) < 0)
                        menor = j;
                }
                if (menor != i)
                    intercambiar(i, menor);
            }
        }

        // ---------- Opciones del menú ----------
        static public void registrar_solicitud()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("****REGISTRAR SOLICITUD****");
            Console.WriteLine("-------------------------------------------------");
            if (con_sol >= max_sol)
            {
                Console.WriteLine("ERROR 001: CAPACIDAD MAXIMA ALCANZADA.");
                return;
            }
            Console.WriteLine("(Escriba 0 en cualquier campo para cancelar)");
            cancelado = false;
            int n = con_sol;

            nombres_sol[n] = pedir_texto("Nombres y apellidos: ");
            tipo_id_sol[n] = pedir_tipo_id();
            num_id_sol[n] = pedir_numero_id(tipo_id_sol[n]);
            telefono_sol[n] = pedir_telefono();
            correo_sol[n] = pedir_correo();
            postulante_sol[n] = pedir_texto("Nombre del postulante: ");
            tipo_sol[n] = pedir_opcion("Tipo de documento:", tipos_doc, "tipo", "004");

            if (!cancelado)
            {
                int dup = buscar_duplicado(n);
                if (dup >= 0)
                {
                    Console.WriteLine($"ERROR 013: YA EXISTE UNA SOLICITUD IGUAL ({codigo_sol[dup]}) PARA ESTE DOCUMENTO DE IDENTIDAD, POSTULANTE Y TIPO.");
                    return;
                }
            }

            asunto_sol[n] = pedir_texto("Asunto: ");
            nivel_sol[n] = "";
            tercio_sol[n] = "";
            justificacion_sol[n] = "";
            if (es_certificado(n))
            {
                nivel_sol[n] = pedir_opcion("Nivel de estudio:", niveles_estudio, "nivel", "011");
                tercio_sol[n] = pedir_tercio();
                justificacion_sol[n] = pedir_texto("Justificación: ");
            }

            if (cancelado)
            {
                Console.WriteLine("Registro cancelado. No se guardó nada.");
                return;
            }

            codigo_sol[n] = "C-" + (n + 1).ToString("000");
            fecha_sol[n] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            estado_sol[n] = estados[0];
            mostrar_cargo(n);
            guardar_cargo(n);
            con_sol++;
            guardar_datos();
            Console.WriteLine("Solicitud recibida correctamente.");
        }

        static public void mostrar_resumen(int i)
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Código: {codigo_sol[i]} | Estado: {estado_sol[i]} | Fecha: {fecha_sol[i]}");
            Console.WriteLine($"Nombre: {nombres_sol[i]} | {tipo_id_sol[i]}: {num_id_sol[i]}");
            Console.WriteLine($"Teléfono: {telefono_sol[i]} | Correo: {correo_sol[i]}");
            Console.WriteLine($"Postulante: {postulante_sol[i]}");
            Console.WriteLine($"Documento: {tipo_sol[i]}");
            Console.WriteLine($"Asunto: {asunto_sol[i]}");
            if (es_certificado(i))
            {
                Console.WriteLine($"Nivel: {nivel_sol[i]} | Tercio superior: {tercio_sol[i]}");
                Console.WriteLine($"Justificación: {justificacion_sol[i]}");
            }
            Console.WriteLine("-------------------------------------------------");
        }

        static public void mostrar()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("****SOLICITUDES RECIBIDAS****");
            Console.WriteLine("-------------------------------------------------");
            if (con_sol == 0)
            {
                Console.WriteLine("ERROR 005: NO HAY SOLICITUDES POR MOSTRAR.");
                return;
            }
            for (int i = 0; i < con_sol; i++)
            {
                mostrar_resumen(i);
            }
            Console.WriteLine("Total: " + con_sol);
        }

        static public void buscar()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("****BUSCAR SOLICITUD****");
            Console.WriteLine("-------------------------------------------------");
            if (con_sol == 0)
            {
                Console.WriteLine("ERROR 005: NO HAY SOLICITUDES POR MOSTRAR.");
                return;
            }
            Console.WriteLine("(Escriba 0 para cancelar)");
            cancelado = false;
            string termino = pedir_texto("Buscar por código, documento de identidad, solicitante o postulante: ");
            if (cancelado)
            {
                Console.WriteLine("Búsqueda cancelada.");
                return;
            }
            int encontrados = 0;
            for (int i = 0; i < con_sol; i++)
            {
                if (contiene(codigo_sol[i], termino) || contiene(num_id_sol[i], termino)
                    || contiene(nombres_sol[i], termino) || contiene(postulante_sol[i], termino))
                {
                    mostrar_resumen(i);
                    encontrados++;
                }
            }
            if (encontrados == 0)
                Console.WriteLine("ERROR 017: NO SE ENCONTRARON SOLICITUDES PARA \"" + termino + "\".");
            else
                Console.WriteLine("Encontradas: " + encontrados);
        }

        static public void cambiar_estado()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("****CAMBIAR ESTADO****");
            Console.WriteLine("-------------------------------------------------");
            if (con_sol == 0)
            {
                Console.WriteLine("ERROR 005: NO HAY SOLICITUDES POR MOSTRAR.");
                return;
            }
            Console.WriteLine("(Escriba 0 para cancelar)");
            cancelado = false;
            string codigo = pedir_texto("Código de la solicitud (ej: C-001): ");
            if (cancelado)
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            int i = buscar_codigo(codigo);
            if (i < 0)
            {
                Console.WriteLine("ERROR 018: NO EXISTE LA SOLICITUD " + codigo.ToUpper() + ".");
                return;
            }
            Console.WriteLine($"Estado actual de {codigo_sol[i]}: {estado_sol[i]}");
            string nuevo = pedir_opcion("Estado:", estados, "estado", "014");
            if (cancelado)
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            estado_sol[i] = nuevo;
            guardar_datos();
            guardar_cargo(i);
            Console.WriteLine($"Estado actualizado: {codigo_sol[i]} -> {estado_sol[i]}");
        }

        static public void ordenar_solicitudes()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("****ORDENAR SOLICITUDES****");
            Console.WriteLine("-------------------------------------------------");
            if (con_sol == 0)
            {
                Console.WriteLine("ERROR 005: NO HAY SOLICITUDES POR MOSTRAR.");
                return;
            }
            Console.WriteLine("(Escriba 0 para cancelar)");
            cancelado = false;
            string elegido = pedir_opcion("Ordenar por:", criterios_orden, "criterio", "019");
            if (cancelado)
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            int criterio = Array.IndexOf(criterios_orden, elegido) + 1;
            ordenar(criterio);
            guardar_datos();
            Console.WriteLine("Solicitudes ordenadas por: " + elegido);
            mostrar();
        }

        static void Main(string[] args)
        {
            cargar_datos();

            int opc = 0;
            while (opc != 6)
            {
                Console.Clear();
                titulo();
                Console.WriteLine("[1] Registrar solicitud");
                Console.WriteLine("[2] Mostrar solicitudes recibidas");
                Console.WriteLine("[3] Buscar solicitud");
                Console.WriteLine("[4] Cambiar estado de una solicitud");
                Console.WriteLine("[5] Ordenar solicitudes");
                Console.WriteLine("[6] Salir");
                Console.Write("Ingrese una opción: ");
                if (!int.TryParse(Console.ReadLine(), out opc))
                {
                    Console.WriteLine("ERROR 006: OPCIÓN INVÁLIDA. DEBE INGRESAR UN NÚMERO.");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }
                switch (opc)
                {
                    case 1:
                        registrar_solicitud();
                        break;
                    case 2:
                        mostrar();
                        break;
                    case 3:
                        buscar();
                        break;
                    case 4:
                        cambiar_estado();
                        break;
                    case 5:
                        ordenar_solicitudes();
                        break;
                    case 6:
                        Console.WriteLine("Saliendo del programa...");
                        break;
                    default:
                        Console.WriteLine("ERROR 007: OPCIÓN INVÁLIDA. DEBE INGRESAR UN NÚMERO ENTRE 1 Y 6.");
                        break;
                }
                Console.ReadKey();
            }
        }
    }
}
