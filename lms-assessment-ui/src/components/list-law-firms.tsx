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
import Skeleton from "@mui/material/Skeleton";

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
      {status === "error" && <p>Error loading law firms.</p>}
      {status !== "error" && (
        <TableContainer component={Paper}>
          <Table sx={{ minWidth: 500 }}>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell style={{ width: 160 }} align="right">
                  Phone number
                </TableCell>
                <TableCell style={{ width: 160 }} align="right">
                  Email
                </TableCell>
                <TableCell style={{ width: 160 }} align="right">
                  Created At
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {status === "pending" &&
                // render rows of skeletons to avoid layout shift when data loads
                [...Array(pageSize)].map((_, index) => (
                  <TableRow key={index}>
                    <TableCell colSpan={3} align="center">
                      <Skeleton />
                    </TableCell>
                  </TableRow>
                ))}
              {status === "success" &&
                data.items.map((row) => (
                  <TableRow key={row.name}>
                    <TableCell scope="row">{row.name}</TableCell>
                    <TableCell align="right">{row.phoneNumber}</TableCell>
                    <TableCell align="right">{row.email}</TableCell>
                    <TableCell align="right">
                      {new Date(row.createdAt).toLocaleString("en-GB", {
                        dateStyle: "short",
                        timeStyle: "short",
                      })}
                    </TableCell>
                  </TableRow>
                ))}
            </TableBody>
            <TableFooter>
              <TableRow>
                <TablePagination
                  rowsPerPageOptions={[5, 10, 20]}
                  count={data?.totalCount ?? 0}
                  rowsPerPage={pageSize}
                  page={page}
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
