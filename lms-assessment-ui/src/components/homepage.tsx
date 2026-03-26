import Container from "@mui/material/Container";
import Stack from "@mui/material/Stack";
import CreateLawFirmForm from "./create-law-firm";
import ListLawFirms from "./list-law-firms";

function Homepage() {
  return (
    <Container maxWidth="md">
      <header>
        <h1>LMS.Assessment</h1>
      </header>
      <Stack component="main" spacing={4}>
        <ListLawFirms />
        <CreateLawFirmForm />
      </Stack>
    </Container>
  );
}

export default Homepage;
