import * as React from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableFooter from "@mui/material/TableFooter";
import TablePagination from "@mui/material/TablePagination";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import useApi from "~/hooks/useApi";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import TableHead from "@mui/material/TableHead";

export default function ListLawFirms() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);

  const { getLawFirms } = useApi();

  const { data, status } = useQuery({
    queryKey: ["getLawFirms", page, pageSize],
    queryFn: () => getLawFirms(page, pageSize),
  });

  const handleChangePage = (
    _event: React.MouseEvent<HTMLButtonElement> | null,
    newPage: number,
  ) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    setPageSize(parseInt(event.target.value, 10));
    setPage(1);
  };

  return (
    <section id="list-firms">
      <h2>List of Law Firms</h2>
      {status === "pending" && <p>Loading...</p>}
      {status === "error" && <p>Error loading law firms.</p>}
      {status === "success" && data.totalCount > 0 && (
        <TableContainer component={Paper}>
          <Table sx={{ minWidth: 500 }}>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell align="right">Phone number</TableCell>
                <TableCell align="right">Email</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.items.map((row) => (
                <TableRow key={row.name}>
                  <TableCell component="th" scope="row">
                    {row.name}
                  </TableCell>
                  <TableCell style={{ width: 160 }} align="right">
                    {row.phoneNumber}
                  </TableCell>
                  <TableCell style={{ width: 160 }} align="right">
                    {row.email}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
            <TableFooter>
              <TableRow>
                <TablePagination
                  rowsPerPageOptions={[5, 10, 20]}
                  colSpan={3}
                  count={data.totalCount}
                  rowsPerPage={pageSize}
                  page={page}
                  slotProps={{
                    select: {
                      inputProps: {
                        "aria-label": "rows per page",
                      },
                      native: true,
                    },
                  }}
                  onPageChange={handleChangePage}
                  onRowsPerPageChange={handleChangeRowsPerPage}
                />
              </TableRow>
            </TableFooter>
          </Table>
        </TableContainer>
      )}
    </section>
  );
}
