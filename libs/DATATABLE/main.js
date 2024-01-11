$(document).ready(function () {
	$('#myTable').DataTable({
		language: {
			"lengthMenu": "Mostrar _MENU_ registros",
			"zeroRecords": "No se encontraron resultados",
			"info": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
			"infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
			"infoFiltered": "(filtrado de un total de _MAX_ registros)",
			"sSearch": "Buscar:",
			"oPaginate": {
				"sFirst": '<button class="btn btn-info btn-sm">PRIMERO</button>',
				"sLast": '<button class="btn btn-info btn-sm">ULTIMO</button>',
				"sNext": '<button class="btn btn-info btn-sm">SIGUIENTE</button>',
				"sPrevious": '<button class="btn btn-info btn-sm">ANTERIOR</button>'
			},
			"sProcessing": "Procesando...",
		},
		//para usar los botones   
		responsive: "true",
		//dom: 'Bfrtilp',
		dom:'lfBrtp',
		buttons: [
			{
				extend: 'excelHtml5',
				text: '<i>excel</i>',
				titleAttr: 'Exportar a Excel',
				className: 'btn btn-success'
			},


		]
	});
});	
